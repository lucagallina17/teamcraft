import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { TeamService } from './team.service';
import { TeamDto } from './team.model';

export const teamDetailResolver: ResolveFn<TeamDto> = (route) => {
    const projectId = route.paramMap.get('projectId')!;
    const id = route.paramMap.get('id')!;

    return inject(TeamService).getById(projectId, id);
};
