import { Routes } from '@angular/router';
import { adminGuard } from './core/services/admin.guard';
import { customerGuard } from './core/services/customer.guard';
import { StoreShellComponent } from './features/store/shell/store-shell.component';
import { HomeComponent } from './home/home.component';
import { CategoryPageComponent } from './home/pages/category-page/category-page.component';
import { ProductPageComponent } from './home/pages/product-page/product-page.component';
import { CartComponent } from './features/store/cart/cart.component';
import { LoginPageComponent } from './home/pages/login-page/login-page.component';
import { TrackingComponent } from './features/store/tracking/tracking.component';
import { CheckoutComponent } from './features/store/checkout/checkout.component';
import { AccountOrdersComponent } from './features/store/account-orders/account-orders.component';
import { AccountLoginComponent } from './features/store/account-login/account-login.component';
import { AccountRegisterComponent } from './features/store/account-register/account-register.component';
import { AccountComponent } from './features/store/account/account.component';
import { LoginComponent } from './features/auth/login/login.component';
import { AdminLayoutComponent } from './features/admin/layout/admin-layout.component';
import { AdminDashboardComponent } from './features/admin/dashboard/admin-dashboard.component';
import { ProductsAdminComponent } from './features/admin/products/products-admin.component';
import { OrdersAdminComponent } from './features/admin/orders/orders-admin.component';
import { PromosAdminComponent } from './features/admin/promos/promos-admin.component';
import { DeliveryAdminComponent } from './features/admin/delivery/delivery-admin.component';
import { UsersAdminComponent } from './features/admin/users/users-admin.component';

export const routes: Routes = [
  {
    path: '',
    component: StoreShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'home' },
      { path: 'home', component: HomeComponent },
      { path: 'category/:slug', component: CategoryPageComponent },
      { path: 'product/:slug', component: ProductPageComponent },
      { path: 'cart', component: CartComponent },
      { path: 'login', component: LoginPageComponent },
      { path: 'tracking', component: TrackingComponent },
      { path: 'checkout', component: CheckoutComponent },
      { path: 'my-orders', component: AccountOrdersComponent, canActivate: [customerGuard] },
      { path: 'account', component: AccountComponent, canActivate: [customerGuard] },
      { path: 'account/login', component: AccountLoginComponent },
      { path: 'account/register', component: AccountRegisterComponent }
    ]
  },
  { path: 'auth/login', component: LoginComponent },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [adminGuard],
    children: [
      { path: '', component: AdminDashboardComponent },
      { path: 'products', component: ProductsAdminComponent },
      { path: 'orders', component: OrdersAdminComponent },
      { path: 'promos', component: PromosAdminComponent },
      { path: 'delivery', component: DeliveryAdminComponent },
      { path: 'users', component: UsersAdminComponent }
    ]
  },
  { path: '**', redirectTo: '/home' }
];
