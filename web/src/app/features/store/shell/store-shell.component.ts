import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TopBarComponent } from '../../../layout/top-bar/top-bar.component';
import { HeaderComponent } from '../../../layout/header/header.component';
import { MainNavComponent } from '../../../layout/main-nav/main-nav.component';
import { BenefitsStripComponent } from '../../../layout/benefits-strip/benefits-strip.component';
import { FooterComponent } from '../../../layout/footer/footer.component';

@Component({
  standalone: true,
  imports: [RouterOutlet, TopBarComponent, HeaderComponent, MainNavComponent, BenefitsStripComponent, FooterComponent],
  templateUrl: './store-shell.component.html',
  styleUrl: './store-shell.component.scss'
})
export class StoreShellComponent {}
