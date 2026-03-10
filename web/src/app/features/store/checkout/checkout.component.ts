import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryZone, UserAddress } from '../../../core/models/store.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatRadioModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit {
  zones: DeliveryZone[] = [];
  slots: any[] = [];
  addresses: UserAddress[] = [];

  error = '';
  saveAddress = false;
  addressLabel = '';
  selectedAddressId = 'new';

  readonly newAddressKey = 'new';

  form: any = {
    customerName: '',
    email: '',
    phone: '',
    deliveryAddress: '',
    deliveryArea: '',
    deliveryZoneId: '',
    deliveryTimeSlotId: '',
    deliveryDate: '',
    promoCode: '',
    paymentMethod: 2
  };

  constructor(
    private readonly api: ApiService,
    public readonly auth: AuthService,
    public readonly cart: CartService,
    private readonly router: Router
  ) {}

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn;
  }

  get usingSavedAddress(): boolean {
    return this.isLoggedIn && this.selectedAddressId !== this.newAddressKey;
  }

  ngOnInit(): void {
    this.prefillUserBasics();

    this.api.getDeliveryZones().subscribe(z => {
      this.zones = z;

      if (this.isLoggedIn) {
        this.loadAddresses();
      }
    });

    this.loadSlots();
  }

  onDeliveryDateChange(): void {
    this.loadSlots();
    this.form.deliveryTimeSlotId = '';
  }

  onAddressSelectionChange(): void {
    this.error = '';

    if (this.selectedAddressId === this.newAddressKey) {
      this.saveAddress = false;
      this.addressLabel = '';
      return;
    }

    const selected = this.addresses.find(x => x.id === this.selectedAddressId);
    if (!selected) {
      return;
    }

    this.form.deliveryAddress = selected.addressLine;
    this.form.deliveryArea = selected.area;
    this.form.phone = selected.phone || this.form.phone;
    this.form.customerName = selected.recipientName || this.form.customerName;

    const matchingZone = this.findMatchingZone(selected.area, selected.emirate);
    if (matchingZone) {
      this.form.deliveryZoneId = matchingZone.id;
    } else {
      this.form.deliveryZoneId = '';
      this.error = `No delivery zone found for ${selected.area}, ${selected.emirate}. Please select a zone manually.`;
    }

    this.saveAddress = false;
    this.addressLabel = '';
  }

  onZoneChange(): void {
    const selectedZone = this.zones.find(z => z.id === this.form.deliveryZoneId);
    if (!selectedZone) {
      return;
    }

    // Keep area aligned with the selected zone to avoid mismatch at checkout.
    this.form.deliveryArea = selectedZone.area;
  }

  onAreaChange(): void {
    const area = (this.form.deliveryArea || '').trim();
    if (!area) {
      return;
    }

    const matchedZone = this.findMatchingZone(area);
    if (matchedZone) {
      this.form.deliveryZoneId = matchedZone.id;
      this.error = '';
    }
  }

  placeOrder(): void {
    this.error = '';

    if (!this.cart.items.length) {
      this.error = 'Your cart is empty.';
      return;
    }

    if (!this.form.deliveryDate || !this.form.deliveryTimeSlotId) {
      this.error = 'Please select a delivery date and time slot.';
      return;
    }

    const payload = {
      userId: this.auth.userId,
      ...this.form,
      deliveryDate: this.form.deliveryDate,
      paymentMethod: Number(this.form.paymentMethod),
      saveAddress: this.isLoggedIn && !this.usingSavedAddress && this.saveAddress,
      addressLabel: this.isLoggedIn && !this.usingSavedAddress && this.saveAddress ? (this.addressLabel || 'Checkout Address') : null,
      items: this.cart.items.map(x => ({
        productId: x.productId,
        productVariantId: x.variantId,
        quantity: x.quantity,
        addOnIds: x.addOns.map(a => a.id),
        messageCard: x.messageCard
      }))
    };

    this.api.checkout(payload).subscribe({
      next: (res: any) => {
        this.cart.clear();
        this.router.navigate(['/tracking'], {
          queryParams: { orderNumber: res.orderNumber, contact: this.form.email }
        });
      },
      error: (err: any) => {
        this.error = err?.error?.message || 'Unable to place order. Please verify delivery area, zone, date and slot.';
      }
    });
  }

  private prefillUserBasics(): void {
    if (!this.isLoggedIn) {
      return;
    }

    this.form.customerName = this.auth.fullName || '';
    this.form.email = this.auth.email || '';
  }

  private loadAddresses(): void {
    this.api.getAccountProfile().subscribe({
      next: profile => {
        this.addresses = profile.addresses || [];

        this.form.customerName = profile.fullName || this.form.customerName;
        this.form.email = profile.email || this.form.email;
        this.form.phone = profile.phone || this.form.phone;

        const defaultAddress = this.addresses.find(x => x.isDefault) || this.addresses[0];
        if (defaultAddress) {
          this.selectedAddressId = defaultAddress.id;
          this.onAddressSelectionChange();
        }
      },
      error: () => {
        this.addresses = [];
      }
    });
  }

  private loadSlots(): void {
    const date = this.form.deliveryDate || undefined;
    this.api.getDeliverySlots(date).subscribe(s => (this.slots = s.filter((x: any) => x.isAvailable !== false)));
  }

  private findMatchingZone(area: string, emirate?: string): DeliveryZone | undefined {
    const areaKey = this.normalize(area);
    const emirateKey = this.normalize(emirate || '');

    return this.zones.find(zone => {
      const areaMatches = this.normalize(zone.area) === areaKey;
      if (!areaMatches) {
        return false;
      }

      if (!emirateKey) {
        return true;
      }

      return this.normalize(zone.emirate) === emirateKey;
    });
  }

  private normalize(value: string): string {
    return (value || '').replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
  }
}
