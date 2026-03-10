import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ApiService } from '../../../core/services/api.service';
import { AccountProfile, UserAddress } from '../../../core/models/store.models';

interface AddressFormModel {
  id?: string;
  label: string;
  recipientName: string;
  phone: string;
  addressLine: string;
  area: string;
  emirate: string;
  landmark: string;
  isDefault: boolean;
}

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss'
})
export class AccountComponent implements OnInit {
  loading = false;
  savingProfile = false;
  savingAddress = false;
  error = '';
  success = '';

  profile: AccountProfile | null = null;
  profileForm = {
    fullName: '',
    phone: ''
  };

  addressForm: AddressFormModel = this.emptyAddressForm();
  editingAddressId: string | null = null;

  readonly emirates = [
    'Dubai',
    'Abu Dhabi',
    'Sharjah',
    'Ajman',
    'Ras Al Khaimah',
    'Fujairah',
    'Umm Al Quwain'
  ];

  constructor(private readonly api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.api.getAccountProfile().subscribe({
      next: data => {
        this.profile = data;
        this.profileForm.fullName = data.fullName || '';
        this.profileForm.phone = data.phone || '';
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Unable to load account details right now.';
      }
    });
  }

  saveProfile(): void {
    if (!this.profileForm.fullName.trim() || !this.profileForm.phone.trim()) {
      this.error = 'Name and phone are required.';
      this.success = '';
      return;
    }

    this.savingProfile = true;
    this.error = '';
    this.success = '';
    this.api
      .updateAccountProfile({
        fullName: this.profileForm.fullName.trim(),
        phone: this.profileForm.phone.trim()
      })
      .subscribe({
        next: () => {
          this.savingProfile = false;
          this.success = 'Profile updated.';
          this.load();
        },
        error: () => {
          this.savingProfile = false;
          this.error = 'Unable to update profile.';
        }
      });
  }

  editAddress(address: UserAddress): void {
    this.editingAddressId = address.id;
    this.addressForm = {
      id: address.id,
      label: address.label,
      recipientName: address.recipientName,
      phone: address.phone,
      addressLine: address.addressLine,
      area: address.area,
      emirate: address.emirate,
      landmark: address.landmark || '',
      isDefault: address.isDefault
    };
    this.error = '';
    this.success = '';
  }

  resetAddressForm(): void {
    this.editingAddressId = null;
    this.addressForm = this.emptyAddressForm();
  }

  saveAddress(): void {
    if (!this.isAddressFormValid()) {
      this.error = 'Please fill all required address fields.';
      this.success = '';
      return;
    }

    const payload = {
      label: this.addressForm.label.trim(),
      recipientName: this.addressForm.recipientName.trim(),
      phone: this.addressForm.phone.trim(),
      addressLine: this.addressForm.addressLine.trim(),
      area: this.addressForm.area.trim(),
      emirate: this.addressForm.emirate.trim(),
      landmark: this.addressForm.landmark.trim(),
      isDefault: this.addressForm.isDefault
    };

    this.savingAddress = true;
    this.error = '';
    this.success = '';

    const request$ = this.editingAddressId
      ? this.api.updateAddress(this.editingAddressId, payload)
      : this.api.createAddress(payload);

    request$.subscribe({
      next: () => {
        this.savingAddress = false;
        this.success = this.editingAddressId ? 'Address updated.' : 'Address added.';
        this.resetAddressForm();
        this.load();
      },
      error: () => {
        this.savingAddress = false;
        this.error = 'Unable to save address.';
      }
    });
  }

  deleteAddress(id: string): void {
    this.error = '';
    this.success = '';
    this.api.deleteAddress(id).subscribe({
      next: () => {
        if (this.editingAddressId === id) {
          this.resetAddressForm();
        }
        this.success = 'Address removed.';
        this.load();
      },
      error: () => {
        this.error = 'Unable to remove this address.';
      }
    });
  }

  private isAddressFormValid(): boolean {
    return !!(
      this.addressForm.label.trim() &&
      this.addressForm.recipientName.trim() &&
      this.addressForm.phone.trim() &&
      this.addressForm.addressLine.trim() &&
      this.addressForm.area.trim() &&
      this.addressForm.emirate.trim()
    );
  }

  private emptyAddressForm(): AddressFormModel {
    return {
      label: '',
      recipientName: '',
      phone: '',
      addressLine: '',
      area: '',
      emirate: 'Dubai',
      landmark: '',
      isDefault: false
    };
  }
}
