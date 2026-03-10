import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '../../../core/services/api.service';

@Component({
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatChipsModule],
  templateUrl: './account-orders.component.html',
  styleUrl: './account-orders.component.scss'
})
export class AccountOrdersComponent implements OnInit {
  orders: any[] = [];
  selected: any | null = null;

  constructor(private readonly api: ApiService) {}

  ngOnInit(): void {
    this.api.getMyOrders().subscribe(data => {
      this.orders = data;
      if (data.length > 0) {
        this.view(data[0].id);
      }
    });
  }

  view(orderId: string): void {
    this.api.getMyOrderDetail(orderId).subscribe(data => (this.selected = data));
  }

  itemLineTotal(item: any): number {
    const addOnTotal = (item.addOns || []).reduce((sum: number, addOn: any) => sum + (addOn.unitPrice || 0), 0);
    return (item.unitPrice + addOnTotal) * item.quantity;
  }
}
