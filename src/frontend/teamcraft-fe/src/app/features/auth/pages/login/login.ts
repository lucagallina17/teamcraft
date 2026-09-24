import { ChangeDetectionStrategy, Component, DestroyRef, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../auth.service';
import { LoginDto } from '../../auth.model';
import { NotificationService } from '../../../../shared/services/notification.service';

const REMEMBERED_EMAIL_KEY = 'teamcraft_remembered_email';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink, InputTextModule, ButtonModule, CardModule, CheckboxModule],
    templateUrl: './login.html',
    styleUrls: ['./login.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly authService = inject(AuthService);
    private readonly fb = inject(FormBuilder);
    private readonly destroyRef = inject(DestroyRef);
    private readonly notificationService = inject(NotificationService);

    private readonly rememberedEmail = localStorage.getItem(REMEMBERED_EMAIL_KEY);

    form = this.fb.group({
        email: [this.rememberedEmail ?? '', [Validators.required, Validators.email]],
        password: ['', Validators.required],
        rememberMe: [this.rememberedEmail !== null]
    });

    onSubmit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const { email, password, rememberMe } = this.form.getRawValue();
        const dto: LoginDto = { email: email!, password: password! };

        this.authService.login(dto)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: () => {
                    if (rememberMe) {
                        localStorage.setItem(REMEMBERED_EMAIL_KEY, dto.email);
                    } else {
                        localStorage.removeItem(REMEMBERED_EMAIL_KEY);
                    }

                    this.notificationService.success('Accesso effettuato con successo');
                    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/employees';
                    this.router.navigateByUrl(returnUrl);
                },
                error: () => {
                    this.notificationService.error('Email o password non corretti');
                }
            });
    }
}
