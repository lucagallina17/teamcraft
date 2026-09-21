import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ColleaguePairDto, SetAffinityDto } from './affinity.model';

@Injectable({ providedIn: 'root' })
export class AffinityService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/employee-affinities`;

    getColleagues(): Observable<ColleaguePairDto[]> {
        return this.http.get<ColleaguePairDto[]>(`${this.baseUrl}/colleagues`);
    }

    setAffinity(dto: SetAffinityDto): Observable<ColleaguePairDto> {
        return this.http.post<ColleaguePairDto>(this.baseUrl, dto);
    }
}