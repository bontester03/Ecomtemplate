import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ApiService } from '../../../core/services/api.service';
import { OrderDetailDialogComponent } from './order-detail-dialog/order-detail-dialog.component';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="mb-1">Orders</h1>
        <p class="text-muted mb-0">Review, track, and update order fulfillment.</p>
      </div>
    </div>

    <div class="card mb-4">
      <div class="card-header d-flex align-items-center">
        <i class="bi bi-bag-check me-2"></i>
        Orders List
      </div>
      <div class="card-body border-bottom">
        <div class="row g-3 align-items-center">
          <div class="col-12 col-md-4 d-flex align-items-center gap-2">
            <select class="form-select w-auto" [(ngModel)]="pageSize" aria-label="Entries per page">
              <option [ngValue]="10">10</option>
              <option [ngValue]="25">25</option>
              <option [ngValue]="50">50</option>
            </select>
            <span class="text-muted">entries per page</span>
          </div>
          <div class="col-12 col-md-4 ms-md-auto">
            <input class="form-control" [(ngModel)]="searchTerm" placeholder="Search..." aria-label="Search orders" />
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th class="ps-4">Order</th>
                <th>Customer</th>
                <th>Total</th>
                <th>Status</th>
                <th class="text-end pe-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let order of filteredOrders">
                <td class="ps-4 fw-semibold">{{ order.orderNumber }}</td>
                <td>{{ order.customerName }}</td>
                <td>AED {{ order.totalAmount }}</td>
                <td>
                  <span class="badge bg-primary-soft text-primary">{{ order.status }}</span>
                </td>
                <td class="text-end pe-4">
                  <button class="btn btn-sm btn-outline-primary" type="button" (click)="viewDetails(order)">View Details</button>
                </td>
              </tr>
              <tr *ngIf="!filteredOrders.length">
                <td class="ps-4 text-muted" colspan="5">No orders found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class OrdersAdminComponent implements OnInit {
  orders: any[] = [];
  searchTerm = '';
  pageSize = 10;

  constructor(private readonly api: ApiService, private readonly dialog: MatDialog) {}

  get filteredOrders(): any[] {
    const query = this.searchTerm.trim().toLowerCase();
    const rows = !query
      ? this.orders
      : this.orders.filter(
          o =>
            String(o.orderNumber || '')
              .toLowerCase()
              .includes(query) ||
            String(o.customerName || '')
              .toLowerCase()
              .includes(query)
        );

    return rows.slice(0, this.pageSize);
  }

  ngOnInit(): void {
    this.loadOrders();
  }

  viewDetails(order: any): void {
    this.api.adminOrderDetail(order.id).subscribe(detail => {
      const ref = this.dialog.open(OrderDetailDialogComponent, {
        width: '980px',
        data: { order: detail }
      });

      ref.afterClosed().subscribe(changed => {
        if (changed) {
          this.loadOrders();
        }
      });
    });
  }

  private loadOrders(): void {
    this.api.adminOrders().subscribe(data => (this.orders = data));
  }
}
