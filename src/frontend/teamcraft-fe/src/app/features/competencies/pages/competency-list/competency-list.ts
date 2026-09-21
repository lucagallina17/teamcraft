import { Component, DestroyRef, inject, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { CompetencyDto } from '../../competency.model';
import { Router } from '@angular/router';
import { concatMap } from 'rxjs';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { CompetencyService } from '../../competency.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-competency-list',
  imports: [TableModule, ButtonModule, CardModule],
  templateUrl: './competency-list.html',
  styleUrl: './competency-list.scss',
})
export class CompetencyList {
  private readonly router = inject(Router);
  private readonly confirmService = inject(ConfirmService);
  private readonly competencyService = inject(CompetencyService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly notificationService = inject(NotificationService);

  competencies = signal<CompetencyDto[]>([]);

  constructor() {
    this._loadCompetencies();
  }

  private _loadCompetencies(): void {
    this.competencyService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => this.competencies.set(data)
      });
  }

  onView(competency: CompetencyDto): void {
    this.router.navigate(['/competencies', competency.id]);
  }

  onNew(): void {
    this.router.navigate(['/competencies', 'new']);
  }

  async onDelete(competency: CompetencyDto): Promise<void> {
    const confirmed = await this.confirmService.confirmDelete(
      `Sei sicuro di voler eliminare la competenza ${competency.name} di tipo ${competency.type}?`
    );

    if (!confirmed) return;

    this.competencyService.delete(competency.id).pipe(
      concatMap(() => this.competencyService.getAll()),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => {
        this.competencies.set(data);
        this.notificationService.success('Competenza eliminata con successo');
      }
    });
  }
}
