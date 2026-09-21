import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateProjectRoleDto, ProjectRoleDto } from './project-role.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProjectRoleService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/projectroles`;

    getById(id: string): Observable<ProjectRoleDto> {
        return this.http.get<ProjectRoleDto>(`${this.baseUrl}/${id}`);
    }

    getAll(): Observable<ProjectRoleDto[]> {
        return this.http.get<ProjectRoleDto[]>(this.baseUrl);
    }

    create(dto: CreateProjectRoleDto): Observable<ProjectRoleDto> {
        return this.http.post<ProjectRoleDto>(this.baseUrl, dto);
    }

    update(id: string, dto: CreateProjectRoleDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}