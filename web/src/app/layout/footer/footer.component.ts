import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  readonly columns = [
    { title: 'INFORMATION', links: ['About Us', 'Contact Us', 'Delivery Policy', 'Terms & Conditions'] },
    { title: 'Services', links: ['Same Day Delivery', 'Midnight Delivery', 'Corporate Flowers', 'International Orders'] },
    { title: 'Occasion', links: ['Birthday Flowers', 'Anniversary Flowers', 'Get Well Soon', 'Congratulations'] },
    { title: 'Flowers', links: ['Rose Collection', 'Tulips', 'Lily Bouquets', 'Mixed Bouquets'] },
    { title: 'Cakes & Chocolate', links: ['Chocolate Boxes', 'Premium Cakes', 'Gift Hampers', 'Add-ons'] },
    { title: 'Special Offers', links: ['Deals Under 150 AED', 'Weekend Offers', 'VIP Flowers', 'Gift Sets'] }
  ];
}
