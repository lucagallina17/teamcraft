import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CompetencyDto, CreateCompetencyDto } from './competency.model';

@Injectable({
    providedIn: 'root'
})
export class CompetencyService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/competencies`;

    getAll(): Observable<CompetencyDto[]> {
        return this.http.get<CompetencyDto[]>(this.baseUrl);
    }

    getById(id: string): Observable<CompetencyDto> {
        return this.http.get<CompetencyDto>(`${this.baseUrl}/${id}`);
    }

    create(dto: CreateCompetencyDto): Observable<CompetencyDto> {
        return this.http.post<CompetencyDto>(this.baseUrl, dto);
    }

    update(id: string, dto: CreateCompetencyDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}