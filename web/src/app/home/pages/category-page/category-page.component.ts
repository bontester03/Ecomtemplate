import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductGridComponent } from '../../product-grid/product-grid.component';
import { HomeService } from '../../services/home.service';
import { Product } from '../../services/storefront.models';
import { ApiService } from '../../../core/services/api.service';

@Component({
  standalone: true,
  imports: [CommonModule, ProductGridComponent],
  templateUrl: './category-page.component.html',
  styleUrl: './category-page.component.scss'
})
export class CategoryPageComponent implements OnInit {
  slug = 'our-complete-range';
  heading = 'Our Complete Range';
  products: Product[] = [];

  constructor(
    private readonly route: ActivatedRoute,
    private readonly homeService: HomeService,
    private readonly api: ApiService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.slug = params.get('slug') || 'our-complete-range';
      this.heading = this.slug === 'our-complete-range'
        ? 'Our Complete Range'
        : this.slug.replace(/-/g, ' ').replace(/\b\w/g, (m) => m.toUpperCase());

      this.api.getCategories().subscribe((rows) => {
        const category = rows.find((x) => x.slug === this.slug);
        if (category) {
          this.heading = category.name;
        }
      });

      this.homeService.getProductsByCategory(this.slug).subscribe((data) => (this.products = data));
    });
  }
}
