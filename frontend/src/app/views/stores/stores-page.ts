import { Component, inject, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PagePadder } from '../../components/page-padder/page-padder';
import StoreLocationModel from '../../models/store-location-model';
import PageModel from '@models/page-model';

@Component({
  selector: 'app-stores-page',
  imports: [MatCardModule, PagePadder, RouterLink],
  templateUrl: './stores-page.html',
})
export class StoresPage {
  private readonly activatedRoute = inject(ActivatedRoute);
  storeLocations = signal<PageModel<StoreLocationModel>>(this.activatedRoute.snapshot.data['storeLocations']);

  constructor() {
    this.activatedRoute.data.subscribe(({ storeLocations }) => {
      this.storeLocations.set(storeLocations);
    });
  }
}
