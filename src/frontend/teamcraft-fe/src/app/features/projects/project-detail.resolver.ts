import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { of } from 'rxjs';
import { ProjectService } from './project.service';
import { ProjectDetailDto } from './project.model';

export const projectDetailResolver: ResolveFn<ProjectDetailDto | null> = (route) => {
    const id = route.paramMap.get('id');

    // se il path è "new" non c'è nulla da caricare
    if (id === 'new') {
        return of(null);
    }

    return inject(ProjectService).getById(id!);
};
