import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { of } from 'rxjs';
import { EmployeeService } from './employee.service';
import { EmployeeDetailDto } from './employee.model';

export const employeeDetailResolver: ResolveFn<EmployeeDetailDto | null> = (route) => {
    const id = route.paramMap.get('id');

    // se il path è "new" non c'è nulla da caricare
    if (id === 'new') {
        return of(null);
    }

    return inject(EmployeeService).getById(id!);
};