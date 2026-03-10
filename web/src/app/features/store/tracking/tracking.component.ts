import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule],
  templateUrl: './tracking.component.html',
  styleUrl: './tracking.component.scss'
})
export class TrackingComponent {
  orderNumber = '';
  contact = '';
  result: any;
  error = '';
  loading = false;
  myOrders: any[] = [];

  constructor(private readonly api: ApiService, private readonly auth: AuthService, route: ActivatedRoute) {
    this.orderNumber = route.snapshot.queryParamMap.get('orderNumber') || '';
    this.contact = route.snapshot.queryParamMap.get('contact') || '';

    if (this.orderNumber) {
      this.track();
      return;
    }

    if (this.auth.isLoggedIn) {
      this.api.getMyOrders().subscribe({
        next: orders => {
          this.myOrders = orders;
          if (orders.length === 0) {
            return;
          }

          this.orderNumber = orders[0].orderNumber;
          this.track();
        }
      });
    }
  }

  trackOrderFromList(order: any): void {
    this.orderNumber = order.orderNumber;
    this.contact = '';
    this.track();
  }

  track(): void {
    this.error = '';
    this.result = null;
    this.loading = true;

    const contact = this.contact.trim();
    if (!this.auth.isLoggedIn && !contact) {
      this.error = 'Email or phone is required for guest tracking.';
      this.loading = false;
      return;
    }

    this.api.trackOrder(this.orderNumber.trim(), contact || undefined).subscribe({
      next: res => {
        this.result = res;
        this.loading = false;
      },
      error: () => {
        this.error = 'Order not found. Check order number and contact details.';
        this.loading = false;
      }
    });
  }
}
