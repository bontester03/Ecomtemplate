import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiConfig } from '../models/app-config';
import {
  AccountProfile,
  Category,
  DeliveryTimeSlot,
  DeliveryZone,
  OrderSummary,
  ProductDetail,
  ProductListItem
} from '../models/store.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = apiConfig.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/catalog/categories`);
  }

  getProducts(search = '', categoryId = '', featuredOnly?: boolean, sortBy = ''): Observable<ProductListItem[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (categoryId) params = params.set('categoryId', categoryId);
    if (featuredOnly === true) params = params.set('featuredOnly', 'true');
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<ProductListItem[]>(`${this.baseUrl}/catalog/products`, { params });
  }

  getProduct(slug: string): Observable<ProductDetail> {
    return this.http.get<ProductDetail>(`${this.baseUrl}/catalog/products/${slug}`);
  }

  getDeliveryZones(): Observable<DeliveryZone[]> {
    return this.http.get<DeliveryZone[]>(`${this.baseUrl}/meta/delivery-zones`);
  }

  getDeliverySlots(deliveryDate?: string): Observable<any[]> {
    let params = new HttpParams();
    if (deliveryDate) {
      params = params.set('deliveryDate', deliveryDate);
    }

    return this.http.get<any[]>(`${this.baseUrl}/meta/delivery-slots`, { params });
  }

  trackOrder(orderNumber: string, contact?: string): Observable<any> {
    let params = new HttpParams().set('orderNumber', orderNumber);
    if (contact && contact.trim()) {
      params = params.set('contact', contact.trim());
    }

    return this.http.get(`${this.baseUrl}/orders/track`, { params });
  }

  getMyOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(`${this.baseUrl}/orders/my`);
  }

  getMyOrderDetail(orderId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/orders/my/${orderId}`);
  }

  login(payload: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/login`, payload);
  }

  register(payload: { fullName: string; email: string; phone: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/auth/register`, payload);
  }

  checkout(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/orders/checkout`, payload);
  }

  adminOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/admin/orders`);
  }

  adminUsers(search = ''): Observable<any[]> {
    let params = new HttpParams();
    if (search.trim()) {
      params = params.set('q', search.trim());
    }

    return this.http.get<any[]>(`${this.baseUrl}/admin/users`, { params });
  }

  adminUpdateUserAccess(id: string, isActive: boolean): Observable<any> {
    return this.http.patch(`${this.baseUrl}/admin/users/${id}/access`, { isActive });
  }

  adminOrderDetail(orderId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/orders/${orderId}`);
  }

  adminUpdateOrderStatus(orderId: string, status: string): Observable<any> {
    return this.http.patch(`${this.baseUrl}/orders/${orderId}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  adminSalesReport(from: string, to: string): Observable<any> {
    const params = new HttpParams().set('from', from).set('to', to);
    return this.http.get<any>(`${this.baseUrl}/admin/reports/sales`, { params });
  }

  adminProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/admin/products`);
  }

  adminCreateProduct(payload: {
    categoryId: string;
    name: string;
    description: string;
    isFeatured: boolean;
    isActive: boolean;
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/admin/products`, payload);
  }

  adminUpdateProduct(
    id: string,
    payload: {
      categoryId: string;
      name: string;
      description: string;
      isFeatured: boolean;
      isActive: boolean;
    }
  ): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/products/${id}`, payload);
  }

  adminDeleteProduct(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/admin/products/${id}`);
  }

  adminProductConfig(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/admin/products/${id}/config`);
  }

  adminUpdateProductConfig(
    id: string,
    payload: {
      variants: { id?: string; name: string; price: number; isDefault: boolean; isActive: boolean }[];
      addOnIds: string[];
    }
  ): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/products/${id}/config`, payload);
  }

  adminPromos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/admin/promocodes`);
  }

  adminCreatePromo(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/admin/promocodes`, payload);
  }

  adminUpdatePromo(id: string, payload: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/promocodes/${id}`, payload);
  }

  adminDeletePromo(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/admin/promocodes/${id}`);
  }

  adminZones(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/admin/delivery-zones`);
  }

  adminCreateZone(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/admin/delivery-zones`, payload);
  }

  adminUpdateZone(id: string, payload: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/delivery-zones/${id}`, payload);
  }

  adminDeleteZone(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/admin/delivery-zones/${id}`);
  }

  adminSlots(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/admin/delivery-slots`);
  }

  adminCreateSlot(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/admin/delivery-slots`, payload);
  }

  adminUpdateSlot(id: string, payload: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/delivery-slots/${id}`, payload);
  }

  adminDeleteSlot(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/admin/delivery-slots/${id}`);
  }

  adminDeliverySettings(): Observable<any> {
    return this.http.get(`${this.baseUrl}/admin/delivery-settings`);
  }

  adminUpdateDeliverySettings(payload: { sameDayCutoffHour: number }): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/delivery-settings`, payload);
  }

  getAccountProfile(): Observable<AccountProfile> {
    return this.http.get<AccountProfile>(`${this.baseUrl}/account/profile`);
  }

  updateAccountProfile(payload: { fullName: string; phone: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/account/profile`, payload);
  }

  createAddress(payload: {
    label: string;
    recipientName: string;
    phone: string;
    addressLine: string;
    area: string;
    emirate: string;
    landmark?: string;
    isDefault: boolean;
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/account/addresses`, payload);
  }

  updateAddress(
    id: string,
    payload: {
      label: string;
      recipientName: string;
      phone: string;
      addressLine: string;
      area: string;
      emirate: string;
      landmark?: string;
      isDefault: boolean;
    }
  ): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/account/addresses/${id}`, payload);
  }

  deleteAddress(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/account/addresses/${id}`);
  }
}

