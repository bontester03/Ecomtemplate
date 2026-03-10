import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-rating-stars',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rating-stars.component.html',
  styleUrl: './rating-stars.component.scss'
})
export class RatingStarsComponent {
  @Input() rating = 5;
  @Input() sizeClass = '';

  readonly stars = Array.from({ length: 5 }, (_, i) => i + 1);
}
