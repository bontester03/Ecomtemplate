import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import { ProductDetail, ProductListItem } from '../../core/models/store.models';
import { Product, Review } from './storefront.models';

@Injectable({ providedIn: 'root' })
export class HomeService {
  private readonly reviews: Review[] = [
    { rating: 5, title: 'Beautiful flowers and exact match', message: 'Arrangement looked exactly like photos. Fast same-day delivery.', reviewer: 'Ayesha Khan', date: 'March 2026' },
    { rating: 5, title: 'Perfect anniversary surprise', message: 'Fresh blooms and clear delivery updates. Great support on WhatsApp.', reviewer: 'Omar R.', date: 'February 2026' },
    { rating: 5, title: 'Reliable and premium quality', message: 'Ordered last minute and the bouquet still arrived on time.', reviewer: 'Noura S.', date: 'January 2026' }
  ];

  constructor(private readonly api: ApiService) {}

  getFeaturedProducts(): Observable<Product[]> {
    return this.api.getProducts('', '', true).pipe(map((rows) => rows.map((x) => this.mapListItemToProduct(x))));
  }

  getProductsByCategory(slug: string): Observable<Product[]> {
    if (slug === 'our-complete-range' || slug === 'all') {
      return this.api.getProducts('', '', false).pipe(map((rows) => rows.map((x) => this.mapListItemToProduct(x))));
    }

    return this.api.getCategories().pipe(
      switchMap((categories) => {
        const category = categories.find((x) => x.slug === slug);
        if (!category) {
          return of([] as Product[]);
        }

        return this.api.getProducts('', category.id, false).pipe(map((rows) => rows.map((x) => this.mapListItemToProduct(x, slug))));
      })
    );
  }

  getProductBySlug(slug: string): Observable<Product | undefined> {
    return this.api.getProduct(slug).pipe(map((item) => this.mapDetailToProduct(item)));
  }

  getReviews(): Observable<Review[]> {
    return of(this.reviews);
  }

  private mapListItemToProduct(item: ProductListItem, categorySlug = ''): Product {
    return {
      id: item.id,
      title: item.name,
      slug: item.slug,
      categorySlug,
      categoryName: item.categoryName,
      imageUrl: item.imageUrl || 'assets/images/product-1.svg',
      priceFrom: item.startingPrice
    };
  }

  private mapDetailToProduct(item: ProductDetail): Product {
    return {
      id: item.id,
      title: item.name,
      slug: item.slug,
      categoryName: item.categoryName,
      imageUrl: item.imageUrls?.[0] || 'assets/images/product-1.svg',
      priceFrom: item.variants?.[0]?.price || 0,
      description: item.description
    };
  }
}
