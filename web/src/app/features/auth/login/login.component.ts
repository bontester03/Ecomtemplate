import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  readonly currentYear = new Date().getFullYear();
  email = '';
  password = '';
  error = '';

  constructor(private readonly api: ApiService, private readonly auth: AuthService, private readonly router: Router) {}

  submit(): void {
    this.api.login({ email: this.email, password: this.password }).subscribe({
      next: res => {
        this.auth.saveAuth(res);
        if (res.role !== 'Admin' && res.role !== 'Staff') {
          this.router.navigateByUrl('/account/login');
          return;
        }

        this.router.navigateByUrl('/admin');
      },
      error: () => (this.error = 'Invalid login credentials')
    });
  }
}
