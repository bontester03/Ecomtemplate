import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCheckboxModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>{{ data.mode === 'edit' ? 'Edit Promo Code' : 'Create Promo Code' }}</h2>
    <mat-dialog-content>
      <mat-form-field appearance="outline">
        <mat-label>Code</mat-label>
        <input matInput [(ngModel)]="form.code" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Type</mat-label>
        <mat-select [(ngModel)]="form.type">
          <mat-option [value]="1">Fixed Amount</mat-option>
          <mat-option [value]="2">Percentage</mat-option>
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Value</mat-label>
        <input matInput type="number" min="0" [(ngModel)]="form.value" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Min Spend</mat-label>
        <input matInput type="number" min="0" [(ngModel)]="form.minSpend" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Expiry</mat-label>
        <input matInput type="datetime-local" [(ngModel)]="expiresLocal" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Usage Limit (optional)</mat-label>
        <input matInput type="number" min="1" [(ngModel)]="form.usageLimit" />
      </mat-form-field>

      <mat-checkbox [(ngModel)]="form.isActive">Active</mat-checkbox>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="ref.close()">Cancel</button>
      <button mat-raised-button color="primary" [disabled]="!canSave()" (click)="save()">Save</button>
    </mat-dialog-actions>
  `
})
export class PromoDialogComponent {
  form: any = {
    code: '',
    type: 2,
    value: 10,
    minSpend: 0,
    usageLimit: null,
    isActive: true
  };

  expiresLocal = '';

  constructor(public ref: MatDialogRef<PromoDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    if (data.promo) {
      this.form = {
        code: data.promo.code,
        type: Number(data.promo.type),
        value: Number(data.promo.value),
        minSpend: Number(data.promo.minSpend),
        usageLimit: data.promo.usageLimit,
        isActive: !!data.promo.isActive
      };

      const d = new Date(data.promo.expiresAtUtc);
      const pad = (n: number) => String(n).padStart(2, '0');
      this.expiresLocal = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
    } else {
      const d = new Date();
      d.setDate(d.getDate() + 30);
      const pad = (n: number) => String(n).padStart(2, '0');
      this.expiresLocal = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
    }
  }

  canSave(): boolean {
    return !!this.form.code?.trim() && !!this.expiresLocal;
  }

  save(): void {
    this.ref.close({
      code: this.form.code.trim().toUpperCase(),
      type: Number(this.form.type),
      value: Number(this.form.value),
      minSpend: Number(this.form.minSpend),
      expiresAtUtc: new Date(this.expiresLocal).toISOString(),
      usageLimit: this.form.usageLimit ? Number(this.form.usageLimit) : null,
      isActive: !!this.form.isActive
    });
  }
}
