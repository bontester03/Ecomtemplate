export interface Category {
  id: string;
  name: string;
  slug: string;
  isFeatured: boolean;
}

export interface ProductListItem {
  id: string;
  name: string;
  slug: string;
  categoryName: string;
  startingPrice: number;
  imageUrl?: string;
  isFeatured: boolean;
}

export interface ProductVariant {
  id: string;
  name: string;
  price: number;
  isDefault: boolean;
}

export interface AddOn {
  id: string;
  name: string;
  price: number;
}

export interface ProductDetail {
  id: string;
  name: string;
  slug: string;
  description: string;
  categoryName: string;
  variants: ProductVariant[];
  addOns: AddOn[];
  imageUrls: string[];
}

export interface DeliveryZone {
  id: string;
  emirate: string;
  area: string;
  charge: number;
}

export interface DeliveryTimeSlot {
  id: string;
  label: string;
  startTime: string;
  endTime: string;
}

export interface OrderSummary {
  id: string;
  orderNumber: string;
  createdAtUtc: string;
  totalAmount: number;
  status: string;
  isPaid: boolean;
}

export interface UserAddress {
  id: string;
  label: string;
  recipientName: string;
  phone: string;
  addressLine: string;
  area: string;
  emirate: string;
  landmark?: string | null;
  isDefault: boolean;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}

export interface AccountProfile {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  createdAtUtc: string;
  addresses: UserAddress[];
}

