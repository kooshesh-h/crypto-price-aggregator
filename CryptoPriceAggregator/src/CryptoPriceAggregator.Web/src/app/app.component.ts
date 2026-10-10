import { Component } from '@angular/core';
import { PricesPageComponent } from './features/prices/prices-page.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [PricesPageComponent],
  template: '<app-prices-page />'
})
export class AppComponent {
}
