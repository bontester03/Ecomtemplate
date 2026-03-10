import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="mb-1">Users</h1>
        <p class="text-muted mb-0">Registered and guest users with access controls.</p>
      </div>
      <button class="btn btn-outline-primary mt-3 mt-sm-0" type="button" (click)="loadUsers()">
        <i class="bi bi-arrow-clockwise me-1"></i>
        Refresh
      </button>
    </div>

    <div class="card mb-4">
      <div class="card-header d-flex align-items-center">
        <i class="bi bi-people me-2"></i>
        User Directory
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
            <input class="form-control" [(ngModel)]="searchTerm" (ngModelChange)="loadUsers()" placeholder="Search name, email, phone..." aria-label="Search users" />
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th class="ps-4">Name</th>
                <th>Type</th>
                <th>Email / Phone</th>
                <th>Address</th>
                <th>Orders</th>
                <th>Role</th>
                <th>Access</th>
                <th class="text-end pe-4">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let user of pagedUsers">
                <td class="ps-4 fw-semibold">{{ user.fullName || 'N/A' }}</td>
                <td>
                  <span class="badge rounded-pill" [ngClass]="user.userType === 'Guest' ? 'bg-secondary text-white' : 'bg-info text-white'">
                    {{ user.userType }}
                  </span>
                </td>
                <td>
                  <div>{{ user.email || '-' }}</div>
                  <small class="text-muted">{{ user.phone || '-' }}</small>
                </td>
                <td>
                  <div>{{ user.address || '-' }}</div>
                  <small class="text-muted">{{ user.area || '-' }}</small>
                </td>
                <td>{{ user.ordersCount }}</td>
                <td>{{ user.role }}</td>
                <td>
                  <span class="badge rounded-pill" [ngClass]="user.isActive ? 'bg-success text-white' : 'bg-danger text-white'">
                    {{ user.isActive ? 'Enabled' : 'Disabled' }}
                  </span>
                </td>
                <td class="text-end pe-4">
                  <button
                    class="btn btn-sm"
                    [ngClass]="user.isActive ? 'btn-outline-danger' : 'btn-outline-success'"
                    type="button"
                    [disabled]="!user.canToggleAccess || user.busy"
                    (click)="toggleAccess(user)"
                  >
                    {{ user.isActive ? 'Disable' : 'Enable' }}
                  </button>
                </td>
              </tr>
              <tr *ngIf="!pagedUsers.length">
                <td class="ps-4 text-muted" colspan="8">No users found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class UsersAdminComponent implements OnInit {
  users: Array<any & { busy?: boolean }> = [];
  searchTerm = '';
  pageSize = 10;

  constructor(private readonly api: ApiService) {}

  get pagedUsers(): Array<any & { busy?: boolean }> {
    return this.users.slice(0, this.pageSize);
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.api.adminUsers(this.searchTerm).subscribe(rows => (this.users = rows || []));
  }

  toggleAccess(user: any & { busy?: boolean }): void {
    if (!user.canToggleAccess || !user.id) {
      return;
    }

    user.busy = true;
    const nextState = !user.isActive;

    this.api.adminUpdateUserAccess(user.id, nextState).subscribe({
      next: () => {
        user.isActive = nextState;
        user.busy = false;
      },
      error: () => {
        user.busy = false;
      }
    });
  }
}
