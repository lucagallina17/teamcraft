import { Component } from '@angular/core';
import { ToastModule } from 'primeng/toast';

@Component({
    selector: 'app-toast',
    standalone: true,
    imports: [ToastModule],
    template: `
    <p-toast position="bottom-right" />
  `,
    styles: [`
    :host ::ng-deep .p-toast {
      font-family: var(--font-family);
    }

    :host ::ng-deep .p-toast-message {
      border-radius: 16px;
      box-shadow: 0 20px 60px rgba(0,0,0,0.15);
      border: none;
      overflow: hidden;
    }

    :host ::ng-deep .p-toast-message-content {
      padding: 16px 18px;
      gap: 12px;
    }

    :host ::ng-deep .p-toast-summary {
      font-size: 14px;
      font-weight: 600;
      letter-spacing: -0.01em;
    }

    :host ::ng-deep .p-toast-detail {
      font-size: 13px;
      color: var(--color-text-secondary);
      margin-top: 2px;
    }

    :host ::ng-deep .p-toast-message-success {
      background: white;
      border-left: 4px solid #34c759;
    }

    :host ::ng-deep .p-toast-message-error {
      background: white;
      border-left: 4px solid var(--color-danger);
    }

    :host ::ng-deep .p-toast-message-info {
      background: white;
      border-left: 4px solid var(--color-accent);
    }

    :host ::ng-deep .p-toast-icon-close {
      color: var(--color-text-secondary);
      border-radius: 8px;
    }

    :host ::ng-deep .p-toast-icon-close:hover {
      background: #f0f0f2;
    }
  `]
})
export class Toast { }