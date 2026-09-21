import { Injectable, inject } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    private readonly messageService = inject(MessageService);

    success(message: string, title: string = 'Fatto'): void {
        this.messageService.add({
            severity: 'success',
            summary: title,
            detail: message,
            life: 4000
        });
    }

    error(message: string, title: string = 'Errore'): void {
        this.messageService.add({
            severity: 'error',
            summary: title,
            detail: message,
            life: 6000 // gli errori restano visibili più a lungo dei successi
        });
    }

    info(message: string, title: string = 'Info'): void {
        this.messageService.add({
            severity: 'info',
            summary: title,
            detail: message,
            life: 4000
        });
    }
}