import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { AffinityService } from './affinity.service';
import { ColleaguePairDto } from './affinity.model';

export const affinityListResolver: ResolveFn<ColleaguePairDto[]> = () => {
    return inject(AffinityService).getColleagues();
};
