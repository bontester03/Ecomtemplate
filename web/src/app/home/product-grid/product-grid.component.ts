import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Product } from '../services/storefront.models';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';

@Component({
  selector: 'app-product-grid',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  templateUrl: './product-grid.component.html',
  styleUrl: './product-grid.component.scss'
})
export class ProductGridComponent {
  @Input() title = 'Feature Products';
  @Input({ required: true }) products: Product[] = [];
}
