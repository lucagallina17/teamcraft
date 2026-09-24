import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { concatMap } from 'rxjs/operators';
import { ProjectService } from '../../project.service';
import { ProjectDto, ProjectStatus } from '../../project.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { COMMON_PIPES } from '../../../../shared/common-imports';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [TableModule, ButtonModule, CardModule, TagModule, ...COMMON_PIPES],
  templateUrl: './project-list.html',
  styleUrls: ['./project-list.scss']
})
export class ProjectList {
  private readonly route = inject(ActivatedRoute);
  private readonly projectService = inject(ProjectService);
  private readonly router = inject(Router);
  private readonly confirmService = inject(ConfirmService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  projects = signal<ProjectDto[]>(this.route.snapshot.data['projects']);

  private loadProjects(): void {
    this.projectService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.projects.set(data)
      });
  }

  statusLabel(status: ProjectStatus): string {
    const labels: Record<ProjectStatus, string> = {
      [ProjectStatus.Draft]: 'Draft',
      [ProjectStatus.Active]: 'Active',
      [ProjectStatus.Completed]: 'Completed',
      [ProjectStatus.Cancelled]: 'Cancelled'
    };
    return labels[status];
  }

  statusSeverity(status: ProjectStatus): 'secondary' | 'success' | 'info' | 'danger' {
    const severities: Record<ProjectStatus, 'secondary' | 'success' | 'info' | 'danger'> = {
      [ProjectStatus.Draft]: 'secondary',
      [ProjectStatus.Active]: 'success',
      [ProjectStatus.Completed]: 'info',
      [ProjectStatus.Cancelled]: 'danger'
    };
    return severities[status];
  }

  onView(project: ProjectDto): void {
    this.router.navigate(['/projects', project.id]);
  }

  onNew(): void {
    this.router.navigate(['/projects', 'new']);
  }

  async onDelete(project: ProjectDto): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare il progetto "${project.name}"?`
    );

    if (!confirmed) return;

    this.projectService.delete(project.id).pipe(
      concatMap(() => this.projectService.getAll()),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => {
        this.projects.set(data);
        this.notificationService.success('Progetto eliminato');
      },
      error: (err) => {
        this.notificationService.error(
          err.error?.message ?? 'Impossibile eliminare il progetto.'
        );
      }
    });
  }
}