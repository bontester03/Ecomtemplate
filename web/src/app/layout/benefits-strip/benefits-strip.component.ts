import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-benefits-strip',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './benefits-strip.component.html',
  styleUrl: './benefits-strip.component.scss'
})
export class BenefitsStripComponent {
  readonly benefits = [
    { icon: 'bi-flower1', text: 'Fresh Flowers' },
    { icon: 'bi-image', text: 'Flowers Same Like Photo' },
    { icon: 'bi-patch-check', text: 'Proof of Delivery' },
    { icon: 'bi-people', text: '11000+ Customers' },
    { icon: 'bi-shield-lock', text: 'Complete Privacy' },
    { icon: 'bi-clock', text: 'Same Day Delivery' },
    { icon: 'bi-cash-coin', text: 'Starting From 90 AED' }
  ];
}
