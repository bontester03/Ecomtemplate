import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule],
  templateUrl: './account-login.component.html',
  styleUrl: './account-login.component.scss'
})
export class AccountLoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(private readonly api: ApiService, private readonly auth: AuthService, private readonly router: Router) {}

  submit(): void {
    this.error = '';
    this.api.login({ email: this.email, password: this.password }).subscribe({
      next: res => {
        this.auth.saveAuth(res);
        if (res.role === 'Admin' || res.role === 'Staff') {
          this.router.navigateByUrl('/admin');
          return;
        }

        this.router.navigateByUrl('/my-orders');
      },
      error: () => (this.error = 'Invalid login credentials')
    });
  }
}
