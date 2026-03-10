import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { Category } from '../../core/models/store.models';

@Component({
  selector: 'app-sidebar-filters',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './sidebar-filters.component.html',
  styleUrl: './sidebar-filters.component.scss'
})
export class SidebarFiltersComponent {
  allCategories: Category[] = [];
  featuredCategories: Category[] = [];

  constructor(private readonly api: ApiService) {
    this.api.getCategories().subscribe((rows) => {
      this.allCategories = rows;
      this.featuredCategories = rows.filter((x) => x.isFeatured);
    });
  }
}
