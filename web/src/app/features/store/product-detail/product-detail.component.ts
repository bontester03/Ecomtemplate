import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../../core/services/api.service';
import { CartService } from '../../../core/services/cart.service';
import { ProductDetail } from '../../../core/models/store.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatCheckboxModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  product?: ProductDetail;
  selectedVariantId = '';
  quantity = 1;
  messageCard = '';
  selectedAddOns = new Set<string>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly api: ApiService,
    private readonly cart: CartService,
    private readonly router: Router,
    @Inject(DOCUMENT) private readonly document: Document
  ) {}

  ngOnInit(): void {
    const slug = this.route.snapshot.paramMap.get('slug') || '';
    this.api.getProduct(slug).subscribe(p => {
      this.product = p;
      this.selectedVariantId = p.variants.find(x => x.isDefault)?.id ?? p.variants[0]?.id ?? '';
      this.injectProductJsonLd(p);
    });
  }

  toggleAddOn(addOnId: string, checked: boolean): void {
    if (checked) this.selectedAddOns.add(addOnId);
    else this.selectedAddOns.delete(addOnId);
  }

  addToCart(): void {
    if (!this.product || !this.selectedVariantId) return;

    const variant = this.product.variants.find(v => v.id === this.selectedVariantId);
    if (!variant) return;

    const addOns = this.product.addOns.filter(a => this.selectedAddOns.has(a.id));

    this.cart.add({
      productId: this.product.id,
      productName: this.product.name,
      slug: this.product.slug,
      variantId: variant.id,
      variantName: variant.name,
      unitPrice: variant.price,
      quantity: this.quantity,
      addOns,
      messageCard: this.messageCard,
      imageUrl: this.product.imageUrls[0]
    });

    this.router.navigateByUrl('/cart');
  }

  private injectProductJsonLd(p: ProductDetail): void {
    const scriptId = 'product-jsonld';
    const existing = this.document.getElementById(scriptId);
    if (existing) existing.remove();

    const jsonLd = {
      '@context': 'https://schema.org',
      '@type': 'Product',
      name: p.name,
      description: p.description,
      image: p.imageUrls,
      offers: p.variants.map(v => ({
        '@type': 'Offer',
        priceCurrency: 'AED',
        price: v.price,
        availability: 'https://schema.org/InStock'
      }))
    };

    const script = this.document.createElement('script');
    script.id = scriptId;
    script.type = 'application/ld+json';
    script.text = JSON.stringify(jsonLd);
    this.document.head.appendChild(script);
  }
}

