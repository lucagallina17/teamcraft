import { Routes } from '@angular/router';
import { employeeDetailResolver } from './features/employees/employee-detail.resolver';
import { competencyDetailResolver } from './features/competencies/competency-detail.resolver';
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
                loadComponent: () =>
                    import('./features/projects/pages/project-list/project-list')
                        .then(m => m.ProjectList)
            },
            {
                path: ':id',
                loadComponent: () =>
                    import('./features/projects/pages/project-detail/project-detail')
                        .then(m => m.ProjectDetail)
            },
            {
                path: ':projectId/teams/proposals',
                loadComponent: () =>
                    import('./features/teams/pages/team-proposals/team-proposals')
                        .then(m => m.TeamProposals)
            },
            {
                path: ':projectId/teams/:id',
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
                loadComponent: () =>
                    import('./features/projects/pages/project-role-list/project-role-list')
                        .then(m => m.ProjectRoleList)
            },
            {
                path: ':id',
                loadComponent: () =>
                    import('./features/projects/pages/project-role-detail/project-role-detail')
                        .then(m => m.ProjectRoleDetail)
            }
        ]
    },
    {
        path: 'affinities',
        canActivate: [authGuard],
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