import { Routes } from '@angular/router';
import { employeeDetailResolver } from './features/employees/employee-detail.resolver';
import { employeeListResolver } from './features/employees/employee-list.resolver';
import { affinityListResolver } from './features/employees/affinity-list.resolver';
import { competencyDetailResolver } from './features/competencies/competency-detail.resolver';
import { competencyListResolver } from './features/competencies/competency-list.resolver';
import { projectListResolver } from './features/projects/project-list.resolver';
import { projectDetailResolver } from './features/projects/project-detail.resolver';
import { projectRoleListResolver } from './features/projects/project-role-list.resolver';
import { projectRoleDetailResolver } from './features/projects/project-role-detail.resolver';
import { teamListByProjectResolver } from './features/teams/team-list.resolver';
import { teamDetailResolver } from './features/teams/team-detail.resolver';
import { teamProposalsResolver } from './features/teams/team-proposals.resolver';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'employees',
        pathMatch: 'full'
    },
    {
        path: 'employees',
        canActivate: [authGuard],
        children: [
            {
                path: '',
                resolve: { employees: employeeListResolver },
                loadComponent: () =>
                    import('./features/employees/pages/employee-list/employee-list')
                        .then(m => m.EmployeeList)
            },
            {
                path: ':id',
                resolve: { employee: employeeDetailResolver },
                loadComponent: () =>
                    import('./features/employees/pages/employee-detail/employee-detail')
                        .then(m => m.EmployeeDetail)
            }
        ]
    },
    {
        path: 'competencies',
        canActivate: [authGuard],
        children: [
            {
                path: '',
                resolve: { competencies: competencyListResolver },
                loadComponent: () =>
                    import('./features/competencies/pages/competency-list/competency-list')
                        .then(m => m.CompetencyList)
            },
            {
                path: ':id',
                resolve: { competency: competencyDetailResolver },
                loadComponent: () =>
                    import('./features/competencies/pages/competency-detail/competency-detail')
                        .then(m => m.CompetencyDetail)
            }
        ]
    },
    {
        path: 'projects',
        canActivate: [authGuard],
        children: [
            {
                path: '',
                resolve: { projects: projectListResolver },
                loadComponent: () =>
                    import('./features/projects/pages/project-list/project-list')
                        .then(m => m.ProjectList)
            },
            {
                path: ':id',
                resolve: {
                    project: projectDetailResolver,
                    roles: projectRoleListResolver,
                    competencies: competencyListResolver,
                    teams: teamListByProjectResolver
                },
                loadComponent: () =>
                    import('./features/projects/pages/project-detail/project-detail')
                        .then(m => m.ProjectDetail)
            },
            {
                path: ':projectId/teams/proposals',
                resolve: { proposals: teamProposalsResolver },
                loadComponent: () =>
                    import('./features/teams/pages/team-proposals/team-proposals')
                        .then(m => m.TeamProposals)
            },
            {
                path: ':projectId/teams/:id',
                resolve: { team: teamDetailResolver },
                loadComponent: () =>
                    import('./features/teams/pages/team-detail/team-detail')
                        .then(m => m.TeamDetail)
            },
        ]
    },
    {
        path: 'project-roles',
        canActivate: [authGuard],
        children: [
            {
                path: '',
                resolve: { roles: projectRoleListResolver },
                loadComponent: () =>
                    import('./features/projects/pages/project-role-list/project-role-list')
                        .then(m => m.ProjectRoleList)
            },
            {
                path: ':id',
                resolve: { role: projectRoleDetailResolver },
                loadComponent: () =>
                    import('./features/projects/pages/project-role-detail/project-role-detail')
                        .then(m => m.ProjectRoleDetail)
            }
        ]
    },
    {
        path: 'affinities',
        canActivate: [authGuard],
        resolve: { pairs: affinityListResolver },
        loadComponent: () =>
            import('./features/employees/pages/affinity-list/affinity-list')
                .then(m => m.AffinityList)
    },
    {
        path: 'login',
        loadComponent: () =>
            import('./features/auth/pages/login/login')
                .then(m => m.Login)
    },
    {
        path: 'register',
        loadComponent: () =>
            import('./features/auth/pages/register/register')
                .then(m => m.Register)
    },
    {
        path: '**',
        redirectTo: 'projects'
    }
];