import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import StoreLocationModel from '../../../models/store-location-model';
import PageModel from '@models/page-model';

@Component({
  selector: 'app-stores',
  imports: [MatButtonModule, MatTableModule, RouterLink],
  templateUrl: './stores.html',
  styleUrl: './stores.css',
})
export class Stores {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  storeLocations = signal<PageModel<StoreLocationModel>>(
    this.route.snapshot.data['storeLocations']
  );
  columnsToDisplay = ['displayName', 'address', 'actions'];

  constructor() {
    this.route.data.subscribe((data) => {
      this.storeLocations.set(data['storeLocations'] as PageModel<StoreLocationModel>);
    });
  }

  view(store: StoreLocationModel): void {
    this.router.navigate([`/admin/store/${store.storeLocationId}`]);
  }

  edit(store: StoreLocationModel): void {
    console.log('edit', store);
  }

  delete(store: StoreLocationModel): void {
    console.log('delete', store);
  }
}
