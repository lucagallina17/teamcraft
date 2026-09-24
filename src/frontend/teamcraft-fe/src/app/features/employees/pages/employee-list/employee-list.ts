import { Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { EmployeeService } from '../../employee.service';
import { EmployeeDto } from '../../employee.model';
import { concatMap, map } from 'rxjs';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { CardModule } from 'primeng/card';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [TableModule, ButtonModule, TagModule, CardModule],
  templateUrl: './employee-list.html',
  styleUrls: ['./employee-list.scss']
})
export class EmployeeList {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly confirmService = inject(ConfirmService);
  private readonly employeeService = inject(EmployeeService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  employees = signal<EmployeeDto[]>(this.route.snapshot.data['employees']);

  private _loadEmployees(): void {
    this.employeeService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.employees.set(data)
      });
  }

  onView(employee: EmployeeDto): void {
    this.router.navigate(['/employees', employee.id]);
  }

  onNew(): void {
    this.router.navigate(['/employees', 'new']);
  }

  async onDelete(employee: EmployeeDto): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare ${employee.firstName} ${employee.lastName}?`
    );

    if (!confirmed) return;

    this.employeeService.delete(employee.id).pipe(
      takeUntilDestroyed(this.destroyRef),
      concatMap(() => this.employeeService.getAll())
    ).subscribe({
      next: (data) => {
        this.employees.set(data);
        this.notificationService.success('Dipendente eliminato con successo');
      },
      error: () => this.notificationService.error('Impossibile eliminare il dipendente: potrebbe essere in uso.')
    });
  }


}