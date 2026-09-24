import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { TeamService } from './team.service';
import { TeamProposalDto } from './team.model';

export const teamProposalsResolver: ResolveFn<TeamProposalDto[]> = (route) => {
    const projectId = route.paramMap.get('projectId')!;
    return inject(TeamService).getProposals(projectId);
};
