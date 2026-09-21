import { Injectable, inject } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

@Injectable({
    providedIn: 'root'
})
export class ConfirmService {
    private readonly confirmationService = inject(ConfirmationService);

    confirmDelete(message: string): Promise<boolean> {
        return new Promise((resolve) => {
            this.confirmationService.confirm({
                message,
                header: 'Conferma eliminazione',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Elimina',
                rejectLabel: 'Annulla',
                accept: () => resolve(true),
                reject: () => resolve(false)
            });
        });
    }

    confirmAction(message: string, header: string = 'Conferma azione'): Promise<boolean> {
        return new Promise((resolve) => {
            this.confirmationService.confirm({
                message,
                header,
                icon: 'pi pi-info-circle',
                acceptLabel: 'Conferma',
                rejectLabel: 'Annulla',
                accept: () => resolve(true),
                reject: () => resolve(false)
            });
        });
    }
}