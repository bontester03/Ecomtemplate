import { Injectable } from '@angular/core';

export interface CartLine {
  productId: string;
  productName: string;
  slug: string;
  variantId: string;
  variantName: string;
  unitPrice: number;
  quantity: number;
  addOns: { id: string; name: string; price: number }[];
  messageCard?: string;
  imageUrl?: string;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly key = 'khf_cart';

  get items(): CartLine[] {
    return JSON.parse(localStorage.getItem(this.key) || '[]') as CartLine[];
  }

  add(item: CartLine): void {
    const items = this.items;
    items.push(item);
    localStorage.setItem(this.key, JSON.stringify(items));
  }

  remove(index: number): void {
    const items = this.items;
    items.splice(index, 1);
    localStorage.setItem(this.key, JSON.stringify(items));
  }

  clear(): void {
    localStorage.removeItem(this.key);
  }

  subtotal(): number {
    return this.items.reduce((sum, line) => {
      const addOnTotal = line.addOns.reduce((a, b) => a + b.price, 0);
      return sum + (line.unitPrice + addOnTotal) * line.quantity;
    }, 0);
  }
}

