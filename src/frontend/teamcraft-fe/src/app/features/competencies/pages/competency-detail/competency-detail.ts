import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal, Type } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { CompetencyService } from '../../competency.service';
import { CompetencyDto, CompetencyType, CreateCompetencyDto } from '../../competency.model';
import { SelectModule } from 'primeng/select';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-competency-detail',
  imports: [ReactiveFormsModule, InputTextModule, SelectModule, ButtonModule, CardModule,],
  templateUrl: './competency-detail.html',
  styleUrl: './competency-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CompetencyDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly competencyService = inject(CompetencyService);
  private readonly confirmService = inject(ConfirmService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  competency = signal<CompetencyDto | null>(
    this.route.snapshot.data['competency']
  );

  competencyTypes: CompetencyType[] = Object.values(CompetencyType);

  isNew = signal<boolean>(this.competency() === null);

  form = this.fb.group({
    name: [this.competency()?.name ?? '', Validators.required],
    type: [this.competency()?.type ?? '', Validators.required],
  });

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto = this.form.getRawValue() as {
      name: string;
      type: string;
    } as CreateCompetencyDto;

    if (this.isNew()) {
      this.competencyService.create(dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.notificationService.success('Competenza creata con successo');
            this.router.navigate(['/competencies']);
          }
        });
    } else {
      const id = this.competency()!.id;
      this.competencyService.update(id, dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.notificationService.success('Competenza modificata con successo');
            this.router.navigate(['/competencies']);
          }
        });
    }
  }

  async onDelete(): Promise<void> {
    const comp = this.competency()!;

    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare la competenza ${comp.name} di tipo ${comp.type}?`
    );

    if (!confirmed) return;

    this.competencyService.delete(comp.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.notificationService.success('Competenza eliminata con successo');
          this.router.navigate(['/competencies']);
        }
      });
  }

  onCancel(): void {
    this.router.navigate(['/competencies']);
  }
}
