import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ProjectService } from '../../project.service';
import { AddRequirementCompetencyDto, ProjectDetailDto, RequirementType } from '../../project.model';
import { ProjectRoleDto } from '../../project-role.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { COMMON_PIPES } from '../../../../shared/common-imports';
import { CompetencyDto } from '../../../competencies/competency.model';
import { TeamDto } from '../../../teams/team.model';
import { NotificationService } from '../../../../shared/services/notification.service';
import { teamStatusLabel, teamStatusSeverity } from '../../../../shared/utils/status.utils';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [
    ReactiveFormsModule, InputTextModule, InputNumberModule, SelectModule,
    ButtonModule, CardModule, TagModule, ...COMMON_PIPES
  ],
  templateUrl: './project-detail.html',
  styleUrls: ['./project-detail.scss']
})
export class ProjectDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly projectService = inject(ProjectService);
  private readonly confirmService = inject(ConfirmService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly id = this.route.snapshot.paramMap.get('id')!;
  private readonly notificationService = inject(NotificationService);

  readonly statusLabel = teamStatusLabel;
  readonly statusSeverity = teamStatusSeverity;

  teams = signal<TeamDto[]>(this.route.snapshot.data['teams']);
  project = signal<ProjectDetailDto | null>(this.route.snapshot.data['project']);
  roles = signal<ProjectRoleDto[]>(this.route.snapshot.data['roles']);
  isNew = signal<boolean>(this.project() === null);
  showAddRequirement = signal<boolean>(false);
  competencies = signal<CompetencyDto[]>(this.route.snapshot.data['competencies']);
  activeRequirementId = signal<string | null>(null);
  editingCompetencyId = signal<string | null>(null);

  private readonly statusTransitions: Record<string, string[]> = {
    Draft: ['Cancelled'],
    Active: ['Completed', 'Cancelled'],
    Completed: [],
    Cancelled: []
  };

  form = this.fb.group({
    name: [this.project()?.name ?? '', Validators.required],
    description: [this.project()?.description ?? ''],
    startDate: [this.project()?.startDate?.substring(0, 10) ?? '', Validators.required],
    endDate: [this.project()?.endDate?.substring(0, 10) ?? '', Validators.required]
  });

  requirementForm = this.fb.group({
    projectRoleId: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]]
  });

  requirementTypes = [
    { label: 'Obbligatoria', value: RequirementType.Required },
    { label: 'Preferibile', value: RequirementType.Preferred }
  ];

  competencyForm = this.fb.group({
    competencyId: ['', Validators.required],
    minimumLevel: [3, [Validators.required, Validators.min(1), Validators.max(5)]],
    weight: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
    requirementType: [RequirementType.Required, Validators.required]
  });

  private loadProject(): void {
    this.projectService.getById(this.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => {
          this.project.set(data);
          this.form.patchValue({
            name: data.name,
            description: data.description,
            startDate: data.startDate.substring(0, 10),
            endDate: data.endDate.substring(0, 10)
          });
        }
      });
  }

  onViewTeam(teamId: string): void {
    this.router.navigate(['/projects', this.id, 'teams', teamId]);
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto = this.form.getRawValue() as {
      name: string; description: string; startDate: string; endDate: string;
    };

    if (this.isNew()) {
      this.projectService.create(dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (project) => {
            this.notificationService.success('Progetto creato con successo');
            this.router.navigate(['/projects']);
          }
        });
    } else {
      this.projectService.update(this.id, dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.notificationService.success('Progetto modificato con successo');
            this.loadProject();
          }
        });
    }
  }

  async onDelete(): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare il progetto "${this.project()?.name}"?`
    );
    if (!confirmed) return;

    this.projectService.delete(this.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.notificationService.success('Progetto eliminato');
          this.router.navigate(['/projects']);
        },
        error: (err) => {
          this.notificationService.error(
            err.error?.message ?? 'Impossibile eliminare il progetto.'
          );
        }
      });
  }

  onAddRequirement(): void {
    if (this.requirementForm.invalid) return;

    const dto = this.requirementForm.getRawValue() as { projectRoleId: string; quantity: number };

    this.projectService.addRequirement(this.id, dto)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.showAddRequirement.set(false);
          this.requirementForm.reset({ quantity: 1 });
          this.loadProject();
        }
      });
  }

  async onRemoveRequirement(requirementId: string): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete('Rimuovere questo ruolo dal progetto?');
    if (!confirmed) return;

    this.projectService.removeRequirement(requirementId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: () => this.loadProject() });
  }

  onGenerateTeam(): void {
    this.router.navigate(['/projects', this.id, 'teams', 'proposals']);
  }

  onCancel(): void {
    this.router.navigate(['/projects']);
  }

  onAddCompetency(requirementId: string): void {
    if (this.competencyForm.invalid) return;

    const dto = this.competencyForm.getRawValue() as AddRequirementCompetencyDto;

    this.projectService.addRequirementCompetency(requirementId, dto)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.activeRequirementId.set(null);
          this.competencyForm.reset({ minimumLevel: undefined, weight: undefined, requirementType: RequirementType.Required });
          this.loadProject();
        }
      });
  }

  onStartEditCompetency(comp: { id: string; competencyId: string; minimumLevel: number; weight: number; requirementType: RequirementType }): void {
    this.editingCompetencyId.set(comp.id);
    this.competencyForm.patchValue({
      competencyId: comp.competencyId,
      minimumLevel: comp.minimumLevel,
      weight: comp.weight,
      requirementType: comp.requirementType
    });
  }

  onUpdateCompetency(id: string): void {
    if (this.competencyForm.invalid) return;

    const dto = this.competencyForm.getRawValue() as AddRequirementCompetencyDto;

    this.projectService.updateRequirementCompetency(id, dto)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.editingCompetencyId.set(null);
          this.loadProject();
        }
      });
  }

  canGenerateTeam(): boolean {
    const currentProject = this.project();
    if (!currentProject) return false;

    // Un progetto concluso o annullato non può generare nuovi team, indipendentemente dallo stato dell'ultimo team
    if (currentProject.status === 'Completed' || currentProject.status === 'Cancelled') {
      return false;
    }

    const teamList = this.teams();
    if (teamList.length === 0) return true;

    const lastTeam = teamList[teamList.length - 1];
    return lastTeam.status === 'Closed';
  }

  activeTeamStatusLabel(): string {
    const teamList = this.teams();
    if (teamList.length === 0) return '';

    const lastTeam = teamList[teamList.length - 1];
    const labels: Record<string, string> = {
      Proposed: 'proposto',
      Active: 'attivo'
    };
    return labels[lastTeam.status] ?? '';
  }

  allowedNextStatuses(): string[] {
    const current = this.project()?.status;
    if (!current) return [];
    return this.statusTransitions[current] ?? [];
  }

  projectStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Draft: 'Draft', Active: 'Active', Completed: 'Completed', Cancelled: 'Cancelled'
    };
    return labels[status] ?? status;
  }

  onChangeStatus(newStatus: string): void {
    this.projectService.updateStatus(this.id, { status: newStatus as any })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.project.update(p => p ? { ...p, status: data.status } : p)
      });
  }
}