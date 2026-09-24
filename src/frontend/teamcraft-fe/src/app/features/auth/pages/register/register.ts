import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { SelectModule } from 'primeng/select';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../auth.service';
import { RegisterDto } from '../../auth.model';
import { NotificationService } from '../../../../shared/services/notification.service';

interface EmployeeOption {
    id: string;
    fullName: string;
}

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink, InputTextModule, ButtonModule, CardModule, SelectModule],
    templateUrl: './register.html',
    styleUrls: ['./register.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class Register {
    private readonly router = inject(Router);
    private readonly authService = inject(AuthService);
    private readonly fb = inject(FormBuilder);
    private readonly destroyRef = inject(DestroyRef);
    private readonly notificationService = inject(NotificationService);

    employees = signal<EmployeeOption[]>([]);

    form = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
    });

    constructor() {
    }

    onSubmit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const { email, password } = this.form.getRawValue();
        const dto: RegisterDto = { email: email!, password: password! };

        this.authService.register(dto)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    this.notificationService.success('Registrazione completata con successo');
                    this.router.navigate(['/employees']);
                },
                error: () => {
                    this.notificationService.error(
                        'Impossibile completare la registrazione: verifica i dati o riprova con un\'altra email'
                    );
                }
            });
    }
}
