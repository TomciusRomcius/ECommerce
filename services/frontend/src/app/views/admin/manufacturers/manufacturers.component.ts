import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, RouterLink } from '@angular/router';
import ManufacturerModel from '../../../models/manufacturer-model';
import { SearchBarComponent } from '@components/search-bar/search-bar.component';

@Component({
  selector: 'app-manufacturers',
  imports: [MatButtonModule, MatTableModule, RouterLink, SearchBarComponent],
  templateUrl: './manufacturers.component.html',
  styleUrl: './manufacturers.component.css',
})
export class ManufacturersComponent {
  private route = inject(ActivatedRoute);
  manufacturers = signal<ManufacturerModel[]>(
    this.route.snapshot.data['manufacturers'] ?? [],
  );
  columnsToDisplay = ['name', 'actions'];

  constructor() {
    this.route.data.subscribe((data) => {
      this.manufacturers.set(data['manufacturers'] as ManufacturerModel[]);
    });
  }

  edit(manufacturer: ManufacturerModel): void {
    // wire up navigation or a dialog
    console.log('edit', manufacturer);
  }

  delete(manufacturer: ManufacturerModel): void {
    // wire up API call, then refresh the list
    console.log('delete', manufacturer);
  }
}
