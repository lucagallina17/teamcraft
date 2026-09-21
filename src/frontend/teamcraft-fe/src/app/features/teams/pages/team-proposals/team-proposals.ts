import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TeamService } from '../../team.service';
import { TeamProposalDto } from '../../team.model';

@Component({
    selector: 'app-team-proposals',
    standalone: true,
    imports: [CardModule, ButtonModule, TagModule],
    templateUrl: './team-proposals.html',
    styleUrls: ['./team-proposals.scss']
})
export class TeamProposals {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly teamService = inject(TeamService);
    private readonly destroyRef = inject(DestroyRef);

    private readonly projectId = this.route.snapshot.paramMap.get('projectId')!;

    proposals = signal<TeamProposalDto[]>([]);

    constructor() {
        this.teamService.getProposals(this.projectId)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (data) => this.proposals.set(data)
            });
    }

    onSelect(proposal: TeamProposalDto): void {
        const dto = {
            members: proposal.members.map(m => ({
                employeeId: m.employeeId,
                projectRoleRequirementId: m.projectRoleRequirementId,
                projectRoleId: m.projectRoleId
            }))
        };

        this.teamService.create(this.projectId, dto)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (team) => this.router.navigate(['/projects', this.projectId, 'teams', team.id])
            });
    }

    onBack(): void {
        this.router.navigate(['/projects', this.projectId]);
    }
}