import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ProjectRoleService } from './project-role.service';
import { ProjectRoleDto } from './project-role.model';

export const projectRoleListResolver: ResolveFn<ProjectRoleDto[]> = () => {
    return inject(ProjectRoleService).getAll();
};
