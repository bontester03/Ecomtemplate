import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { Category } from '../../core/models/store.models';

@Component({
  selector: 'app-main-nav',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './main-nav.component.html',
  styleUrl: './main-nav.component.scss'
})
export class MainNavComponent {
  readonly staticLinks = [{ label: 'Our Complete Range', slug: 'our-complete-range' }];
  dynamicLinks: { label: string; slug: string }[] = [];

  constructor(private readonly api: ApiService) {
    this.api.getCategories().subscribe((rows) => {
      const priority = ['valentines-day-flowers', 'birthday-flowers', 'anniversary-flowers', 'vip-flowers', 'rose-collection'];
      const featuredCategories = rows
        .filter((x) => x.isFeatured && x.slug !== 'our-complete-range')
        .sort((a, b) => this.sortFeatured(a, b, priority));

      this.dynamicLinks = featuredCategories.map((x) => ({ label: x.name, slug: x.slug }));
    });
  }

  private sortFeatured(a: Category, b: Category, priority: string[]): number {
    const ai = priority.indexOf(a.slug);
    const bi = priority.indexOf(b.slug);

    if (ai >= 0 && bi >= 0) return ai - bi;
    if (ai >= 0) return -1;
    if (bi >= 0) return 1;

    return a.name.localeCompare(b.name);
  }
}
