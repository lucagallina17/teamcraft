import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { EmployeeService } from './employee.service';
import { EmployeeDto } from './employee.model';

export const employeeListResolver: ResolveFn<EmployeeDto[]> = () => {
    return inject(EmployeeService).getAll();
};
