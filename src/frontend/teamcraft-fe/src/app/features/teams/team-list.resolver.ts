import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { of } from 'rxjs';
import { TeamService } from './team.service';
import { TeamDto } from './team.model';

// Usato dalla pagina project-detail: 'id' qui è il projectId, dato che il resolver
// è agganciato alla rotta 'projects/:id'.
export const teamListByProjectResolver: ResolveFn<TeamDto[]> = (route) => {
    const projectId = route.paramMap.get('id');

    // progetto non ancora creato: nessun team può esistere
    if (projectId === 'new') {
        return of([]);
    }

    return inject(TeamService).getByProject(projectId!);
};
