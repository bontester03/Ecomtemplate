import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../../core/services/api.service';
import { Category, ProductListItem } from '../../../core/models/store.models';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  categories: Category[] = [];
  featured: ProductListItem[] = [];

  constructor(private readonly api: ApiService) {}

  ngOnInit(): void {
    this.api.getCategories().subscribe(data => (this.categories = data.slice(0, 3)));
    this.api.getProducts('', '').subscribe(data => (this.featured = data.filter(x => x.isFeatured).slice(0, 6)));
  }
}

