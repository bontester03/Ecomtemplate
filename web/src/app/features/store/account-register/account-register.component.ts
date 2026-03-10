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
  templateUrl: './account-register.component.html',
  styleUrl: './account-register.component.scss'
})
export class AccountRegisterComponent {
  fullName = '';
  email = '';
  phone = '';
  password = '';
  error = '';

  constructor(private readonly api: ApiService, private readonly auth: AuthService, private readonly router: Router) {}

  submit(): void {
    this.error = '';
    this.api.register({
      fullName: this.fullName,
      email: this.email,
      phone: this.phone,
      password: this.password
    }).subscribe({
      next: res => {
        this.auth.saveAuth(res);
        this.router.navigateByUrl('/my-orders');
      },
      error: () => (this.error = 'Registration failed. Please check all fields and try again.')
    });
  }
}
