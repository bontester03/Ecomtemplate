import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../../../core/services/api.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatDialogModule, MatFormFieldModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>Order {{ data.order.orderNumber }}</h2>
    <mat-dialog-content>
      <div class="meta-grid">
        <div><strong>Customer:</strong> {{ data.order.customerName }}</div>
        <div><strong>Email:</strong> {{ data.order.email }}</div>
        <div><strong>Phone:</strong> {{ data.order.phone }}</div>
        <div><strong>Status:</strong> {{ data.order.status }}</div>
        <div><strong>Paid:</strong> {{ data.order.isPaid ? 'Yes' : 'No' }}</div>
        <div><strong>Delivery:</strong> {{ data.order.deliveryDate }} · {{ data.order.deliveryTimeSlot }}</div>
        <div class="full"><strong>Address:</strong> {{ data.order.deliveryAddress }}, {{ data.order.deliveryArea }}</div>
      </div>

      <h3>Items</h3>
      <div *ngFor="let item of data.order.items" class="item-card">
        <div><strong>{{ item.productName }}</strong> ({{ item.variantName }}) x{{ item.quantity }}</div>
        <div>Unit AED {{ item.unitPrice }} | Add-ons AED {{ item.addOnTotal }}</div>
        <div *ngIf="item.addOns?.length">Add-ons: {{ item.addOns.join(', ') }}</div>
        <div *ngIf="item.messageCard">Card: {{ item.messageCard }}</div>
      </div>

      <h3>Totals</h3>
      <div class="meta-grid">
        <div>Subtotal: AED {{ data.order.subtotal }}</div>
        <div>Discount: AED {{ data.order.discountAmount }}</div>
        <div>Delivery: AED {{ data.order.deliveryCharge }}</div>
        <div><strong>Total: AED {{ data.order.totalAmount }}</strong></div>
      </div>

      <h3>Status Timeline</h3>
      <ul>
        <li *ngFor="let t of data.order.timeline">{{ t.createdAtUtc | date:'medium' }} - {{ t.status }} <span *ngIf="t.note">({{ t.note }})</span></li>
      </ul>

      <div class="status-update">
        <mat-form-field appearance="outline">
          <mat-label>Update Status</mat-label>
          <mat-select [(ngModel)]="selectedStatus">
            <mat-option *ngFor="let s of statuses" [value]="s">{{ s }}</mat-option>
          </mat-select>
        </mat-form-field>
        <button mat-raised-button color="primary" (click)="updateStatus()">Update</button>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="ref.close(updated)">Close</button>
    </mat-dialog-actions>
  `,
  styles: [
    '.meta-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:.5rem;margin-bottom:.75rem;}',
    '.full{grid-column:1/-1;}',
    '.item-card{padding:.5rem;border:1px solid #ddd;border-radius:8px;margin-bottom:.5rem;}',
    '.status-update{display:flex;gap:.75rem;align-items:center;margin-top:1rem;}'
  ]
})
export class OrderDetailDialogComponent {
  statuses = ['Pending', 'Preparing', 'OutForDelivery', 'Delivered', 'Cancelled', 'Refunded'];
  selectedStatus = '';
  updated = false;

  constructor(
    public ref: MatDialogRef<OrderDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private readonly api: ApiService
  ) {
    this.selectedStatus = data.order.status;
  }

  updateStatus(): void {
    if (!this.selectedStatus || this.selectedStatus === this.data.order.status) return;
    this.api.adminUpdateOrderStatus(this.data.order.id, this.selectedStatus).subscribe(() => {
      this.updated = true;
      this.data.order.status = this.selectedStatus;
    });
  }
}
