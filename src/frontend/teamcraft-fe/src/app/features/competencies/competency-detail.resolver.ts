import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { of } from 'rxjs';
import { CompetencyService } from './competency.service';
import { CompetencyDto } from './competency.model';

export const competencyDetailResolver: ResolveFn<CompetencyDto | null> = (route) => {
    const id = route.paramMap.get('id');

    // se il path è "new" non c'è nulla da caricare
    if (id === 'new') {
        return of(null);
    }

    return inject(CompetencyService).getById(id!);
};