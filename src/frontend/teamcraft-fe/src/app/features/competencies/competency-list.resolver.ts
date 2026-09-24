import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { CompetencyService } from './competency.service';
import { CompetencyDto } from './competency.model';

export const competencyListResolver: ResolveFn<CompetencyDto[]> = () => {
    return inject(CompetencyService).getAll();
};
