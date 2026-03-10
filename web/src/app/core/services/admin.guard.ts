import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn && (auth.role === 'Admin' || auth.role === 'Staff')) {
    return true;
  }

  router.navigateByUrl('/auth/login');
  return false;
};

