import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { ProductDetail } from '../../../core/models/store.models';
import { CartService } from '../../../core/services/cart.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.scss'
})
export class ProductPageComponent implements OnInit {
  product?: ProductDetail;
  selectedVariantId = '';
  quantity = 1;
  messageCard = '';
  selectedAddOnIds: string[] = [];
  successMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly api: ApiService,
    private readonly cart: CartService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug') || '';
      this.api.getProduct(slug).subscribe({
        next: (item) => {
          this.product = item;
          this.selectedVariantId = item.variants.find((x) => x.isDefault)?.id || item.variants[0]?.id || '';
          this.quantity = 1;
          this.messageCard = '';
          this.selectedAddOnIds = [];
          this.successMessage = '';
        },
        error: () => (this.product = undefined)
      });
    });
  }

  isAddOnSelected(id: string): boolean {
    return this.selectedAddOnIds.includes(id);
  }

  toggleAddOn(id: string, checked: boolean): void {
    if (checked) {
      if (!this.selectedAddOnIds.includes(id)) {
        this.selectedAddOnIds = [...this.selectedAddOnIds, id];
      }
      return;
    }

    this.selectedAddOnIds = this.selectedAddOnIds.filter((x) => x !== id);
  }

  addToCart(): void {
    if (!this.product || !this.selectedVariantId) {
      return;
    }

    const variant = this.product.variants.find((x) => x.id === this.selectedVariantId);
    if (!variant) {
      return;
    }

    const addOns = this.product.addOns
      .filter((x) => this.selectedAddOnIds.includes(x.id))
      .map((x) => ({ id: x.id, name: x.name, price: x.price }));

    this.cart.add({
      productId: this.product.id,
      productName: this.product.name,
      slug: this.product.slug,
      variantId: variant.id,
      variantName: variant.name,
      unitPrice: variant.price,
      quantity: Math.max(1, this.quantity),
      addOns,
      messageCard: this.messageCard.trim().slice(0, 250),
      imageUrl: this.product.imageUrls[0] || undefined
    });

    this.successMessage = 'Added to cart. You can continue as guest or login at checkout.';
  }
}
