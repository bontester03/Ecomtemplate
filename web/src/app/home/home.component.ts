import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { HeroComponent } from './hero/hero.component';
import { ReviewsStripComponent } from './reviews-strip/reviews-strip.component';
import { ProductGridComponent } from './product-grid/product-grid.component';
import { SidebarFiltersComponent } from './sidebar-filters/sidebar-filters.component';
import { HomeService } from './services/home.service';
import { Product, Review } from './services/storefront.models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HeroComponent, ReviewsStripComponent, ProductGridComponent, SidebarFiltersComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  products: Product[] = [];
  reviews: Review[] = [];

  constructor(private readonly homeService: HomeService) {}

  ngOnInit(): void {
    this.homeService.getFeaturedProducts().subscribe((data) => (this.products = data));
    this.homeService.getReviews().subscribe((data) => (this.reviews = data));
  }
}
