import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ApiService } from '../../../core/services/api.service';
import { ProductDialogComponent } from './product-dialog/product-dialog.component';
import { ProductConfigDialogComponent } from './product-config-dialog/product-config-dialog.component';
import { Category } from '../../../core/models/store.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './products-admin.component.html',
  styleUrl: './products-admin.component.scss'
})
export class ProductsAdminComponent implements OnInit {
  products: any[] = [];
  categories: Category[] = [];
  searchTerm = '';
  pageSize = 10;

  constructor(private readonly api: ApiService, private readonly dialog: MatDialog) {}

  get filteredProducts(): any[] {
    const query = this.searchTerm.trim().toLowerCase();
    const rows = !query
      ? this.products
      : this.products.filter(
          p =>
            String(p.name || '')
              .toLowerCase()
              .includes(query) ||
            String(p.category || '')
              .toLowerCase()
              .includes(query)
        );

    return rows.slice(0, this.pageSize);
  }

  ngOnInit(): void {
    this.api.getCategories().subscribe(rows => (this.categories = rows));
    this.loadProducts();
  }

  create(): void {
    const dialogRef = this.dialog.open(ProductDialogComponent, {
      width: '520px',
      data: { categories: this.categories, mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.api.adminCreateProduct(result).subscribe(() => this.loadProducts());
    });
  }

  edit(product: any): void {
    const dialogRef = this.dialog.open(ProductDialogComponent, {
      width: '520px',
      data: { categories: this.categories, mode: 'edit', product }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.api.adminUpdateProduct(product.id, result).subscribe(() => this.loadProducts());
    });
  }

  remove(product: any): void {
    if (!confirm(`Delete product "${product.name}"?`)) {
      return;
    }

    this.api.adminDeleteProduct(product.id).subscribe(() => this.loadProducts());
  }

  configure(product: any): void {
    this.api.adminProductConfig(product.id).subscribe(config => {
      const dialogRef = this.dialog.open(ProductConfigDialogComponent, {
        width: '900px',
        data: config
      });

      dialogRef.afterClosed().subscribe(result => {
        if (!result) {
          return;
        }

        this.api.adminUpdateProductConfig(product.id, result).subscribe(() => this.loadProducts());
      });
    });
  }

  private loadProducts(): void {
    this.api.adminProducts().subscribe(rows => (this.products = rows));
  }
}
