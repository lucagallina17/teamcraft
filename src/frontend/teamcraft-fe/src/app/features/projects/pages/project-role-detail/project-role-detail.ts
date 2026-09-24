import { Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ProjectRoleService } from '../../project-role.service';
import { ProjectRoleDto } from '../../project-role.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-project-role-detail',
  standalone: true,
  imports: [ReactiveFormsModule, InputTextModule, ButtonModule, CardModule],
  templateUrl: './project-role-detail.html',
  styleUrls: ['./project-role-detail.scss']
})
export class ProjectRoleDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly projectRoleService = inject(ProjectRoleService);
  private readonly confirmService = inject(ConfirmService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  private readonly id = this.route.snapshot.paramMap.get('id')!;
  private readonly resolvedRole: ProjectRoleDto | null = this.route.snapshot.data['role'];

  isNew = signal<boolean>(this.resolvedRole === null);
  roleName = signal<string>(this.resolvedRole?.name ?? '');

  form = this.fb.group({
    name: [this.resolvedRole?.name ?? '', Validators.required],
    description: [this.resolvedRole?.description ?? '']
  });

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto = this.form.getRawValue() as { name: string; description: string };

    if (this.isNew()) {
      this.projectRoleService.create(dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({ next: () => this.router.navigate(['/project-roles']) });
    } else {
      this.projectRoleService.update(this.id, dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({ next: () => this.router.navigate(['/project-roles']) });
    }
  }

  async onDelete(): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare il ruolo "${this.roleName()}"?`
    );

    if (!confirmed) return;

    this.projectRoleService.delete(this.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.router.navigate(['/project-roles']),
        error: () => {
          this.notificationService.error('Questo ruolo è utilizzato in uno o più progetti e non può essere eliminato.');
        }
      });
  }

  onCancel(): void {
    this.router.navigate(['/project-roles']);
  }
}