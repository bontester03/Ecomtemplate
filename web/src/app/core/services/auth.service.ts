import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  get token(): string | null {
    return localStorage.getItem('khf_access_token');
  }

  get role(): string {
    return localStorage.getItem('khf_role') || 'Customer';
  }

  get fullName(): string {
    return localStorage.getItem('khf_name') || '';
  }

  get email(): string {
    const token = this.token;
    if (!token) {
      return '';
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return payload.email || '';
    } catch {
      return '';
    }
  }

  get isLoggedIn(): boolean {
    return !!this.token;
  }

  get isAdminOrStaff(): boolean {
    return this.role === 'Admin' || this.role === 'Staff';
  }

  get userId(): string | null {
    const token = this.token;
    if (!token) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return payload.nameid || payload.sub || null;
    } catch {
      return null;
    }
  }

  saveAuth(response: any): void {
    localStorage.setItem('khf_access_token', response.accessToken);
    localStorage.setItem('khf_refresh_token', response.refreshToken);
    localStorage.setItem('khf_role', response.role || 'Customer');
    localStorage.setItem('khf_name', response.fullName || '');
  }

  logout(): void {
    localStorage.removeItem('khf_access_token');
    localStorage.removeItem('khf_refresh_token');
    localStorage.removeItem('khf_role');
    localStorage.removeItem('khf_name');
  }
}
