import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTeamFromProposalDto, TeamDto, TeamProposalDto, UpdateTeamStatusDto } from './team.model';

@Injectable({
    providedIn: 'root'
})
export class TeamService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/projects`;

    getByProject(projectId: string): Observable<TeamDto[]> {
        return this.http.get<TeamDto[]>(`${this.baseUrl}/${projectId}/teams`);
    }

    getById(projectId: string, teamId: string): Observable<TeamDto> {
        return this.http.get<TeamDto>(`${this.baseUrl}/${projectId}/teams/${teamId}`);
    }

    getProposals(projectId: string): Observable<TeamProposalDto[]> {
        return this.http.get<TeamProposalDto[]>(`${this.baseUrl}/${projectId}/teams/proposals`);
    }

    create(projectId: string, dto: CreateTeamFromProposalDto): Observable<TeamDto> {
        return this.http.post<TeamDto>(`${this.baseUrl}/${projectId}/teams`, dto);
    }

    updateStatus(projectId: string, teamId: string, dto: UpdateTeamStatusDto): Observable<TeamDto> {
        return this.http.put<TeamDto>(`${this.baseUrl}/${projectId}/teams/${teamId}/status`, dto);
    }

    removeMember(projectId: string, teamId: string, employeeId: string): Observable<TeamDto> {
        return this.http.delete<TeamDto>(`${this.baseUrl}/${projectId}/teams/${teamId}/employees/${employeeId}`);
    }
}