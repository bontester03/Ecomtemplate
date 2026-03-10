import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Category } from '../../../../core/models/store.models';

interface ProductDialogData {
  categories: Category[];
  mode?: 'create' | 'edit';
  product?: any;
}

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  template: `
    <h2 mat-dialog-title>{{ mode === 'edit' ? 'Edit Product' : 'Create Product' }}</h2>
    <mat-dialog-content>
      <mat-form-field appearance="outline">
        <mat-label>Name</mat-label>
        <input matInput [(ngModel)]="name" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Category</mat-label>
        <mat-select [(ngModel)]="categoryId">
          <mat-option *ngFor="let c of data.categories" [value]="c.id">{{ c.name }}</mat-option>
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Description</mat-label>
        <textarea matInput rows="4" [(ngModel)]="description"></textarea>
      </mat-form-field>

      <mat-checkbox [(ngModel)]="isFeatured">Featured product</mat-checkbox>
      <mat-checkbox [(ngModel)]="isActive">Active</mat-checkbox>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="ref.close()">Cancel</button>
      <button mat-raised-button color="primary" [disabled]="!canSave()" (click)="save()">
        {{ mode === 'edit' ? 'Update' : 'Save' }}
      </button>
    </mat-dialog-actions>
  `
})
export class ProductDialogComponent {
  name = '';
  categoryId = '';
  description = '';
  isFeatured = false;
  isActive = true;
  mode: 'create' | 'edit' = 'create';

  constructor(
    public ref: MatDialogRef<ProductDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductDialogData
  ) {
    this.mode = data.mode ?? 'create';
    if (data.categories.length > 0) {
      this.categoryId = data.categories[0].id;
    }

    if (data.product) {
      this.name = data.product.name ?? '';
      this.description = data.product.description ?? '';
      this.categoryId = data.product.categoryId ?? this.categoryId;
      this.isFeatured = !!data.product.isFeatured;
      this.isActive = !!data.product.isActive;
    }
  }

  canSave(): boolean {
    return !!this.name.trim() && !!this.categoryId;
  }

  save(): void {
    this.ref.close({
      categoryId: this.categoryId,
      name: this.name.trim(),
      description: this.description.trim(),
      isFeatured: this.isFeatured,
      isActive: this.isActive
    });
  }
}
