import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-icon-badge',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './icon-badge.component.html',
  styleUrl: './icon-badge.component.scss'
})
export class IconBadgeComponent {
  @Input() icon = 'bi-circle';
  @Input() label = '';
  @Input() badge: number | null = null;
}
