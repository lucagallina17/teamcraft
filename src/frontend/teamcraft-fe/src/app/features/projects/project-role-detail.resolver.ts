import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { of } from 'rxjs';
import { ProjectRoleService } from './project-role.service';
import { ProjectRoleDto } from './project-role.model';

export const projectRoleDetailResolver: ResolveFn<ProjectRoleDto | null> = (route) => {
    const id = route.paramMap.get('id');

    // se il path è "new" non c'è nulla da caricare
    if (id === 'new') {
        return of(null);
    }

    return inject(ProjectRoleService).getById(id!);
};
