import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../../core/services/api.service';
import { Category, ProductListItem } from '../../../core/models/store.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss'
})
export class CatalogComponent implements OnInit {
  categories: Category[] = [];
  products: ProductListItem[] = [];
  search = '';
  categoryId = '';

  constructor(private readonly api: ApiService, private readonly route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(q => {
      this.categoryId = q.get('categoryId') ?? '';
      this.load();
    });

    this.api.getCategories().subscribe(c => (this.categories = c));
  }

  load(): void {
    this.api.getProducts(this.search, this.categoryId).subscribe(p => (this.products = p));
  }
}

