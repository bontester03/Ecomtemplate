import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { IconBadgeComponent } from '../../shared/icon-badge/icon-badge.component';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, IconBadgeComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  currency = 'AED';
  search = '';
  constructor(
    public auth: AuthService,
    private readonly cartService: CartService,
    private readonly router: Router
  ) {}

  get cartCount(): number {
    return this.cartService.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/home');
  }
}
