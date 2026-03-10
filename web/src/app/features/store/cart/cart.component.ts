import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  constructor(public readonly cart: CartService) {}

  remove(index: number): void {
    this.cart.remove(index);
  }

  lineTotal(line: any): number {
    const addOnTotal = line.addOns.reduce((sum: number, addOn: any) => sum + addOn.price, 0);
    return (line.unitPrice + addOnTotal) * line.quantity;
  }

  addOnNames(line: any): string {
    return line.addOns.map((addOn: any) => addOn.name).join(', ');
  }
}

