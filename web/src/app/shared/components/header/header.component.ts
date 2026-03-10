import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { apiConfig } from '../../../core/models/app-config';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, MatToolbarModule, MatButtonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  readonly whatsappLink = `https://wa.me/${apiConfig.whatsappNumber}`;
  readonly instagramLink = apiConfig.instagramUrl;

  constructor(readonly auth: AuthService, private readonly router: Router) {}

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/');
  }
}

