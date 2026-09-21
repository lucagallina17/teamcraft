import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { EmployeeService } from '../../employee.service';
import { AddCompetencyAssessmentDto, AssessmentSource, EmployeeDetailDto } from '../../employee.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { COMMON_PIPES } from '../../../../shared/common-imports';
import { CardModule } from 'primeng/card';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CompetencyService } from '../../../competencies/competency.service';
import { CompetencyDto } from '../../../competencies/competency.model';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { NotificationService } from '../../../../shared/services/notification.service';
import { teamStatusLabel, teamStatusSeverity } from '../../../../shared/utils/status.utils';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [ReactiveFormsModule, InputTextModule, ButtonModule, TableModule, TagModule, CardModule, SelectModule, InputNumberModule, ...COMMON_PIPES],
  templateUrl: './employee-detail.html',
  styleUrls: ['./employee-detail.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly employeeService = inject(EmployeeService);
  private readonly competencyService = inject(CompetencyService);
  private readonly confirmService = inject(ConfirmService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  readonly statusLabel = teamStatusLabel;
  readonly statusSeverity = teamStatusSeverity;

  allCompetencies = signal<CompetencyDto[]>([]);
  showAddCompetency = signal<boolean>(false);
  employee = signal<EmployeeDetailDto | null>(
    this.route.snapshot.data['employee']
  );
  isNew = signal<boolean>(this.employee() === null);

  sources = [
    { label: 'Valutazione HR', value: AssessmentSource.HRAssessment },
    { label: 'Autovalutazione', value: AssessmentSource.SelfAssessment },
    { label: 'Feedback di progetto', value: AssessmentSource.ProjectFeedback }
  ];

  competencyForm = this.fb.group({
    competencyId: ['', Validators.required],
    level: [3, [Validators.required, Validators.min(1), Validators.max(5)]],
    source: [AssessmentSource.HRAssessment, Validators.required],
    date: [new Date().toISOString().substring(0, 10), Validators.required]
  });


  form = this.fb.group({
    firstName: [this.employee()?.firstName ?? '', Validators.required],
    lastName: [this.employee()?.lastName ?? '', Validators.required],
    email: [this.employee()?.email ?? '', [Validators.required, Validators.email]]
  });

  // TODO: DA METTERE NEL RESOLVER
  constructor() {
    this._loadCompetencies();
  }

  private _loadCompetencies(): void {
    this.competencyService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: (data) => this.allCompetencies.set(data) });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto = this.form.getRawValue() as {
      firstName: string;
      lastName: string;
      email: string;
    };

    if (this.isNew()) {
      this.employeeService.create(dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.notificationService.success('Dipendente creato con successo');
            this.router.navigate(['/employees']);
          }
        });
    } else {
      const id = this.employee()!.id;
      this.employeeService.update(id, dto)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.notificationService.success('Dipendente modificato con successo');
            this.router.navigate(['/employees']);
          }
        });
    }
  }

  async onDelete(): Promise<void> {
    const emp = this.employee()!;

    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare ${emp.firstName} ${emp.lastName}?`
    );

    if (!confirmed) return;

    this.employeeService.delete(emp.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.notificationService.success('Dipendente eliminato con successo');
          this.router.navigate(['/employees']);
        }
      });
  }

  onCancel(): void {
    this.router.navigate(['/employees']);
  }

  onAddCompetency(): void {
    if (this.competencyForm.invalid) return;

    const dto = this.competencyForm.getRawValue() as AddCompetencyAssessmentDto;
    const employeeId = this.employee()!.id;

    this.employeeService.addCompetency(employeeId, dto)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.showAddCompetency.set(false);
          this.competencyForm.reset({
            level: 3,
            source: AssessmentSource.HRAssessment,
            date: new Date().toISOString().substring(0, 10)
          });
          this.reloadEmployee();
        }
      });
  }

  async onRemoveCompetency(assessmentId: string): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete('Rimuovere questa competenza dal profilo?');
    if (!confirmed) return;

    this.employeeService.removeCompetency(assessmentId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: () => this.reloadEmployee() });
  }

  onViewTeam(teamId: string): void {
    const entry = this.employee()!.teamHistory.find(t => t.teamId === teamId);
    if (!entry) return;

    this.router.navigate(['/projects', entry.projectId, 'teams', teamId], {
      queryParams: { returnTo: 'employee', employeeId: this.employee()!.id }
    });
  }

  private reloadEmployee(): void {
    this.employeeService.getById(this.employee()!.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: (data) => this.employee.set(data) });
  }
}