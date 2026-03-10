import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Review } from '../services/storefront.models';
import { RatingStarsComponent } from '../../shared/rating-stars/rating-stars.component';

@Component({
  selector: 'app-reviews-strip',
  standalone: true,
  imports: [CommonModule, RatingStarsComponent],
  templateUrl: './reviews-strip.component.html',
  styleUrl: './reviews-strip.component.scss'
})
export class ReviewsStripComponent {
  @Input({ required: true }) reviews: Review[] = [];
}
