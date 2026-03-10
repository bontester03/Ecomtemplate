import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ApiService } from '../../../core/services/api.service';
import { PromoDialogComponent } from './promo-dialog/promo-dialog.component';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="mb-1">Promo Codes</h1>
        <p class="text-muted mb-0">Manage discounts and campaign windows.</p>
      </div>
      <button class="btn btn-primary mt-3 mt-sm-0" type="button" (click)="create()">
        <i class="bi bi-plus-circle me-1"></i>
        Add New Promo
      </button>
    </div>

    <div class="card mb-4">
      <div class="card-header d-flex align-items-center">
        <i class="bi bi-ticket-perforated me-2"></i>
        Promo List
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
            <input class="form-control" [(ngModel)]="searchTerm" placeholder="Search..." aria-label="Search promos" />
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th class="ps-4">Code</th>
                <th>Type</th>
                <th>Value</th>
                <th>Min Spend</th>
                <th>Expires</th>
                <th>Status</th>
                <th class="text-end pe-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let row of filteredPromos">
                <td class="ps-4 fw-semibold">{{ row.code }}</td>
                <td>{{ promoTypeLabel(row.type) }}</td>
                <td>{{ row.value }}</td>
                <td>{{ row.minSpend }}</td>
                <td>{{ row.expiresAtUtc | date:'mediumDate' }}</td>
                <td>
                  <span class="badge" [class.bg-success-soft]="row.isActive" [class.text-success]="row.isActive" [class.bg-danger-soft]="!row.isActive" [class.text-danger]="!row.isActive">
                    {{ row.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="text-end pe-4">
                  <div class="btn-group btn-group-sm" role="group" aria-label="Promo actions">
                    <button class="btn btn-outline-secondary" type="button" (click)="edit(row)">Edit</button>
                    <button class="btn btn-outline-danger" type="button" (click)="remove(row)">Delete</button>
                  </div>
                </td>
              </tr>
              <tr *ngIf="!filteredPromos.length">
                <td class="ps-4 text-muted" colspan="7">No promo codes found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: ['.btn-group .btn{min-width:74px;}']
})
export class PromosAdminComponent implements OnInit {
  rows: any[] = [];
  searchTerm = '';
  pageSize = 10;

  constructor(private readonly api: ApiService, private readonly dialog: MatDialog) {}

  get filteredPromos(): any[] {
    const query = this.searchTerm.trim().toLowerCase();
    const rows = !query
      ? this.rows
      : this.rows.filter(
          p =>
            String(p.code || '')
              .toLowerCase()
              .includes(query) ||
            String(this.promoTypeLabel(p.type)).toLowerCase().includes(query)
        );

    return rows.slice(0, this.pageSize);
  }

  ngOnInit(): void {
    this.load();
  }

  promoTypeLabel(type: number): string {
    return Number(type) === 1 ? 'Fixed' : 'Percentage';
  }

  create(): void {
    const dialogRef = this.dialog.open(PromoDialogComponent, {
      width: '560px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      this.api.adminCreatePromo(result).subscribe(() => this.load());
    });
  }

  edit(promo: any): void {
    const dialogRef = this.dialog.open(PromoDialogComponent, {
      width: '560px',
      data: { mode: 'edit', promo }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      this.api.adminUpdatePromo(promo.id, result).subscribe(() => this.load());
    });
  }

  remove(promo: any): void {
    if (!confirm(`Delete promo code "${promo.code}"?`)) {
      return;
    }

    this.api.adminDeletePromo(promo.id).subscribe(() => this.load());
  }

  private load(): void {
    this.api.adminPromos().subscribe(data => (this.rows = data));
  }
}
