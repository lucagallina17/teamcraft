import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TeamService } from '../../team.service';
import { TeamDto, TeamStatus } from '../../team.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { COMMON_PIPES } from '../../../../shared/common-imports';
import { teamStatusLabel, teamStatusSeverity } from '../../../../shared/utils/status.utils';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-team-detail',
  standalone: true,
  imports: [CardModule, ButtonModule, TagModule, ...COMMON_PIPES],
  templateUrl: './team-detail.html',
  styleUrls: ['./team-detail.scss']
})
export class TeamDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly teamService = inject(TeamService);
  private readonly confirmService = inject(ConfirmService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  private readonly projectId = this.route.snapshot.paramMap.get('projectId')!;
  private readonly teamId = this.route.snapshot.paramMap.get('id')!;

  // Da dove arriva la navigazione — determina il comportamento del bottone "indietro"
  private readonly returnTo = this.route.snapshot.queryParamMap.get('returnTo');
  private readonly returnEmployeeId = this.route.snapshot.queryParamMap.get('employeeId');

  readonly statusLabel = teamStatusLabel;
  readonly statusSeverity = teamStatusSeverity;

  team = signal<TeamDto | null>(null);

  private readonly statusOrder: TeamStatus[] = [
    TeamStatus.Proposed, TeamStatus.Active, TeamStatus.Closed
  ];

  constructor() {
    this.loadTeam();
  }

  private loadTeam(): void {
    this.teamService.getById(this.projectId, this.teamId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: (data) => this.team.set(data) });
  }

  nextStatus(): TeamStatus | null {
    const current = this.team()?.status as TeamStatus;
    const index = this.statusOrder.indexOf(current);
    if (index === -1 || index === this.statusOrder.length - 1) return null;
    return this.statusOrder[index + 1];
  }

  async onAdvanceStatus(next: TeamStatus) {
    const message = this.confirmationMessageFor(next);

    const confirmed = await this.confirmService.confirmAction(message, 'Conferma cambio stato');
    if (!confirmed) return;

    this.teamService.updateStatus(this.projectId, this.teamId, { status: next })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => {
          this.team.set(data);
          this.notificationService.success(`Team passato a "${this.statusLabel(next)}"`);
        }
      });
  }

  async onRemoveMember(employeeId: string, fullName: string): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete(
      `Rimuovere ${fullName} dal team?`
    );
    if (!confirmed) return;

    this.teamService.removeMember(this.projectId, this.teamId, employeeId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: (data) => this.team.set(data) });
  }

  backLabel(): string {
    return this.returnTo === 'employee' ? 'Dipendente' : 'Progetto';
  }

  onBack(): void {
    if (this.returnTo === 'employee' && this.returnEmployeeId) {
      this.router.navigate(['/employees', this.returnEmployeeId]);
    } else {
      this.router.navigate(['/projects', this.projectId]);
    }
  }


  private confirmationMessageFor(next: TeamStatus): string {
    const teamName = this.team()?.projectName ?? 'questo team';

    const messages: Record<TeamStatus, string> = {
      [TeamStatus.Proposed]: `Confermi di voler riportare il team di "${teamName}" allo stato ${TeamStatus.Proposed}?`,
      [TeamStatus.Active]: `Confermi di voler attivare il team di "${teamName}"? Il progetto passerà automaticamente allo stato ${TeamStatus.Active}.`,
      [TeamStatus.Closed]: `Confermi di voler chiudere il team di "${teamName}"? Il progetto resterà nel suo stato attuale.`
    };

    return messages[next];
  }
}