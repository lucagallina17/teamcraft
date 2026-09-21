import { Component } from '@angular/core';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ConfirmDialogModule],
  template: `
    <p-confirmDialog 
      [style]="{ width: '380px' }"
      acceptButtonStyleClass="confirm-btn confirm-btn--danger"
      rejectButtonStyleClass="confirm-btn confirm-btn--secondary" />
  `,
  styles: [`
    :host ::ng-deep .p-confirm-dialog {
      border-radius: 20px;
      border: none;
      box-shadow: 0 20px 60px rgba(0,0,0,0.15);
      overflow: hidden;
      font-family: var(--font-family);
    }

    :host ::ng-deep .p-dialog-header {
      padding: 28px 28px 0 28px;
      border: none;
    }

    :host ::ng-deep .p-dialog-title {
      font-size: 18px;
      font-weight: 600;
      letter-spacing: -0.01em;
      color: var(--color-text-primary);
    }

    :host ::ng-deep .p-dialog-header-icon {
      display: none;
    }

    :host ::ng-deep .p-confirm-dialog-message {
      font-size: 15px;
      color: var(--color-text-secondary);
      margin-left: 0;
      line-height: 1.5;
    }

    :host ::ng-deep .p-confirm-dialog-icon {
      display: none;
    }

    :host ::ng-deep .p-dialog-content {
      padding: 12px 28px 28px 28px;
    }

    :host ::ng-deep .p-dialog-footer {
      padding: 0 28px 28px 28px;
      border: none;
      display: flex;
      gap: 10px;
      justify-content: flex-end;
    }

    :host ::ng-deep .confirm-btn {
      border-radius: 980px;
      font-weight: 500;
      font-size: 14px;
      padding: 9px 20px;
      border: none;
      transition: opacity 0.2s ease, background 0.2s ease;
    }

    :host ::ng-deep .confirm-btn--danger {
      background: var(--color-danger);
      color: white;
    }

    :host ::ng-deep .confirm-btn--danger:hover {
      opacity: 0.85;
    }

    :host ::ng-deep .confirm-btn--secondary {
      background: #f0f0f2;
      color: var(--color-text-primary);
    }

    :host ::ng-deep .confirm-btn--secondary:hover {
      background: #e5e5e7;
    }
  `]
})
export class ConfirmDialog { }