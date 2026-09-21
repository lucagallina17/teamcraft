import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProjectDto, ProjectDetailDto, CreateProjectDto, AddRequirementDto, AddRequirementCompetencyDto, ProjectRoleRequirementDto, UpdateProjectStatusDto } from './project.model';

@Injectable({
    providedIn: 'root'
})
export class ProjectService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/projects`;

    getAll(): Observable<ProjectDto[]> {
        return this.http.get<ProjectDto[]>(this.baseUrl);
    }

    getById(id: string): Observable<ProjectDetailDto> {
        return this.http.get<ProjectDetailDto>(`${this.baseUrl}/${id}`);
    }

    create(dto: CreateProjectDto, createdBy: string): Observable<ProjectDto> {
        return this.http.post<ProjectDto>(`${this.baseUrl}?createdBy=${createdBy}`, dto);
    }

    update(id: string, dto: CreateProjectDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }

    addRequirement(projectId: string, dto: AddRequirementDto): Observable<ProjectRoleRequirementDto> {
        return this.http.post<ProjectRoleRequirementDto>(`${this.baseUrl}/${projectId}/requirements`, dto);
    }

    addRequirementCompetency(requirementId: string, dto: AddRequirementCompetencyDto): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/requirements/${requirementId}/competencies`, dto);
    }

    removeRequirement(requirementId: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/requirements/${requirementId}`);
    }

    updateRequirementCompetency(id: string, dto: AddRequirementCompetencyDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/requirements/competencies/${id}`, dto);
    }

    updateStatus(id: string, dto: UpdateProjectStatusDto): Observable<ProjectDto> {
        return this.http.put<ProjectDto>(`${this.baseUrl}/${id}/status`, dto);
    }
}