import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

interface VariantForm {
  id?: string;
  name: string;
  price: number;
  isDefault: boolean;
  isActive: boolean;
}

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCheckboxModule, MatDialogModule, MatFormFieldModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>Configure {{ data.name }}</h2>
    <mat-dialog-content>
      <h3>Variants</h3>
      <div *ngFor="let variant of variants; let i = index" class="variant-row">
        <mat-form-field appearance="outline">
          <mat-label>Name</mat-label>
          <input matInput [(ngModel)]="variant.name" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Price (AED)</mat-label>
          <input matInput type="number" min="0" [(ngModel)]="variant.price" />
        </mat-form-field>

        <mat-checkbox [(ngModel)]="variant.isDefault">Default</mat-checkbox>
        <mat-checkbox [(ngModel)]="variant.isActive">Active</mat-checkbox>
        <button mat-button color="warn" (click)="removeVariant(i)">Remove</button>
      </div>

      <button mat-stroked-button (click)="addVariant()">Add Variant</button>

      <h3 style="margin-top: 1rem;">Add-ons</h3>
      <div *ngFor="let addOn of data.availableAddOns" class="addon-row">
        <mat-checkbox [ngModel]="selectedAddOnIds.has(addOn.id)" (ngModelChange)="toggleAddOn(addOn.id, $event)">
          {{ addOn.name }} (+AED {{ addOn.price }})
        </mat-checkbox>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="ref.close()">Cancel</button>
      <button mat-raised-button color="primary" (click)="save()">Save Configuration</button>
    </mat-dialog-actions>
  `,
  styles: [
    '.variant-row{display:grid;grid-template-columns:1fr 1fr auto auto auto;gap:.5rem;align-items:center;margin-bottom:.5rem;}',
    '.addon-row{margin:.35rem 0;}'
  ]
})
export class ProductConfigDialogComponent {
  variants: VariantForm[] = [];
  selectedAddOnIds = new Set<string>();

  constructor(
    public ref: MatDialogRef<ProductConfigDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.variants = (data.variants ?? []).map((v: any) => ({
      id: v.id,
      name: v.name,
      price: Number(v.price),
      isDefault: !!v.isDefault,
      isActive: !!v.isActive
    }));

    (data.selectedAddOnIds ?? []).forEach((id: string) => this.selectedAddOnIds.add(id));
  }

  addVariant(): void {
    this.variants.push({ name: 'New Variant', price: 0, isDefault: false, isActive: true });
  }

  removeVariant(index: number): void {
    this.variants.splice(index, 1);
  }

  toggleAddOn(id: string, selected: boolean): void {
    if (selected) this.selectedAddOnIds.add(id);
    else this.selectedAddOnIds.delete(id);
  }

  save(): void {
    const variants = this.variants
      .filter(v => !!v.name.trim())
      .map(v => ({
        id: v.id,
        name: v.name.trim(),
        price: Number(v.price),
        isDefault: !!v.isDefault,
        isActive: !!v.isActive
      }));

    this.ref.close({
      variants,
      addOnIds: Array.from(this.selectedAddOnIds)
    });
  }
}
