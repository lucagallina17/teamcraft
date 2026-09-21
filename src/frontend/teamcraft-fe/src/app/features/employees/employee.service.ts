import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddCompetencyAssessmentDto, CreateEmployeeDto, EmployeeDetailDto, EmployeeDto } from './employee.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class EmployeeService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/employees`;

    getAll(): Observable<EmployeeDto[]> {
        return this.http.get<EmployeeDto[]>(this.baseUrl);
    }

    getById(id: string): Observable<EmployeeDetailDto> {
        return this.http.get<EmployeeDetailDto>(`${this.baseUrl}/${id}`);
    }

    create(dto: CreateEmployeeDto): Observable<EmployeeDto> {
        return this.http.post<EmployeeDto>(this.baseUrl, dto);
    }

    update(id: string, dto: CreateEmployeeDto): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }

    addCompetency(employeeId: string, dto: AddCompetencyAssessmentDto): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/${employeeId}/competencies`, dto);
    }

    removeCompetency(assessmentId: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/competencies/${assessmentId}`);
    }
}