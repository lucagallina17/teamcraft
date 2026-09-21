import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputNumberModule } from 'primeng/inputnumber';
import { AffinityService } from '../../affinity.service';
import { ColleaguePairDto } from '../../affinity.model';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
    selector: 'app-affinity-list',
    standalone: true,
    imports: [ReactiveFormsModule, CardModule, ButtonModule, TagModule, InputNumberModule],
    templateUrl: './affinity-list.html',
    styleUrls: ['./affinity-list.scss']
})
export class AffinityList {
    private readonly affinityService = inject(AffinityService);
    private readonly fb = inject(FormBuilder);
    private readonly destroyRef = inject(DestroyRef);
    private readonly notificationService = inject(NotificationService);

    pairs = signal<ColleaguePairDto[]>([]);
    editingPairId = signal<string | null>(null);

    affinityForm = this.fb.group({
        score: [1, [Validators.required, Validators.min(1), Validators.max(5)]]
    });

    constructor() {
        this.loadPairs();
    }

    private loadPairs(): void {
        this.affinityService.getColleagues()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({ next: (data) => this.pairs.set(data) });
    }

    onStartEdit(pair: ColleaguePairDto): void {
        this.editingPairId.set(pair.employee1Id + pair.employee2Id);
        this.affinityForm.patchValue({ score: pair.score ?? 1 });
    }

    onSaveAffinity(pair: ColleaguePairDto): void {
        if (this.affinityForm.invalid) return;

        const score = this.affinityForm.getRawValue().score!;

        this.affinityService.setAffinity({
            employeeId1: pair.employee1Id,
            employeeId2: pair.employee2Id,
            score
        })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    this.editingPairId.set(null);
                    this.loadPairs();
                    this.notificationService.success('Affinità aggiornata');
                }
            });
    }

    statusLabel(status: string): string {
        const labels: Record<string, string> = {
            Undefined: 'Non definita', Positive: 'Positiva', Neutral: 'Neutra', Negative: 'Negativa'
        };
        return labels[status] ?? status;
    }

    statusSeverity(status: string): 'secondary' | 'success' | 'warn' | 'danger' {
        const severities: Record<string, 'secondary' | 'success' | 'warn' | 'danger'> = {
            Undefined: 'secondary', Positive: 'success', Neutral: 'warn', Negative: 'danger'
        };
        return severities[status] ?? 'secondary';
    }
}