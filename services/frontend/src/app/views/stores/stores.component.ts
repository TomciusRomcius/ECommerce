import { Component, inject, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PagePadderComponent } from '../../components/page-padder/page-padder.component';
import StoreLocationModel from '../../models/store-location-model';
import PageModel from '@models/page-model';

@Component({
  selector: 'app-stores-page',
  imports: [MatCardModule, PagePadderComponent, RouterLink],
  templateUrl: './stores.component.html',
})
export class StoresComponent {
  private readonly activatedRoute = inject(ActivatedRoute);
  storeLocations = signal<PageModel<StoreLocationModel>>(this.activatedRoute.snapshot.data['storeLocations']);

  constructor() {
    this.activatedRoute.data.subscribe(({ storeLocations }) => {
      this.storeLocations.set(storeLocations);
    });
  }
}
