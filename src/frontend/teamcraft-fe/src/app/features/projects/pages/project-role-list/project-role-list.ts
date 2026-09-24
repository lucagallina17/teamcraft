import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ProjectRoleService } from '../../project-role.service';
import { ProjectRoleDto } from '../../project-role.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
    selector: 'app-project-role-list',
    standalone: true,
    imports: [TableModule, ButtonModule, CardModule],
    templateUrl: './project-role-list.html',
    styleUrls: ['./project-role-list.scss']
})
export class ProjectRoleList {
    private readonly route = inject(ActivatedRoute);
    private readonly projectRoleService = inject(ProjectRoleService);
    private readonly router = inject(Router);
    private readonly confirmService = inject(ConfirmService);
    private readonly destroyRef = inject(DestroyRef);
    private readonly notificationService = inject(NotificationService);

    roles = signal<ProjectRoleDto[]>(this.route.snapshot.data['roles']);

    private loadRoles(): void {
        this.projectRoleService.getAll()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({ next: (data) => this.roles.set(data) });
    }

    onView(role: ProjectRoleDto): void {
        this.router.navigate(['/project-roles', role.id]);
    }

    onNew(): void {
        this.router.navigate(['/project-roles', 'new']);
    }

    async onDelete(role: ProjectRoleDto): Promise<void> {
        const confirmed = await this.confirmService.confirmDelete(
            `Sei sicuro di voler eliminare il ruolo "${role.name}"?`
        );

        if (!confirmed) return;

        this.projectRoleService.delete(role.id)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    this.loadRoles();
                    this.notificationService.success('Ruolo eliminato con successo');
                },
                error: () => {
                    // il backend blocca l'eliminazione se il ruolo è referenziato (DeleteBehavior.Restrict)
                    this.notificationService.error('Questo ruolo è utilizzato in uno o più progetti e non può essere eliminato.');
                }
            });
    }
}