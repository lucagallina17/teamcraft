import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ProjectService } from './project.service';
import { ProjectDto } from './project.model';

export const projectListResolver: ResolveFn<ProjectDto[]> = () => {
    return inject(ProjectService).getAll();
};
