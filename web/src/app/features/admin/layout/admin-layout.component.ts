import { CommonModule, DOCUMENT } from '@angular/common';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  private readonly themeCssId = 'sb-admin-pro-theme-css';
  private routeSubscription?: Subscription;
  readonly currentYear = new Date().getFullYear();
  readonly avatarPath = 'assets/sb-admin-pro/assets/img/illustrations/profiles/profile-1.png';

  isSidenavToggled = false;

  constructor(
    public readonly auth: AuthService,
    private readonly router: Router,
    @Inject(DOCUMENT) private readonly document: Document
  ) {}

  ngOnInit(): void {
    this.document.body.classList.add('sb-admin-theme', 'nav-fixed');
    this.ensureThemeCss();
    this.syncSidebarClass();

    this.routeSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        if (window.innerWidth < 992) {
          this.isSidenavToggled = false;
          this.syncSidebarClass();
        }
      });
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.document.body.classList.remove('sb-admin-theme', 'nav-fixed', 'sidenav-toggled');
    this.document.getElementById(this.themeCssId)?.remove();
  }

  toggleSidebar(): void {
    this.isSidenavToggled = !this.isSidenavToggled;
    this.syncSidebarClass();
  }

  onMainAreaClick(): void {
    if (window.innerWidth < 992 && this.isSidenavToggled) {
      this.isSidenavToggled = false;
      this.syncSidebarClass();
    }
  }

  isActive(route?: string, exact = false): boolean {
    if (!route) {
      return false;
    }

    return exact ? this.router.url === route : this.router.url.startsWith(route);
  }

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/auth/login');
  }

  private syncSidebarClass(): void {
    this.document.body.classList.toggle('sidenav-toggled', this.isSidenavToggled);
  }

  private ensureThemeCss(): void {
    if (this.document.getElementById(this.themeCssId)) {
      return;
    }

    const link = this.document.createElement('link');
    link.id = this.themeCssId;
    link.rel = 'stylesheet';
    link.href = 'assets/sb-admin-pro/css/styles.css';
    this.document.head.appendChild(link);
  }
}
