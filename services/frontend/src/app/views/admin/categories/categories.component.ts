import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, RouterLink } from '@angular/router';
import CategoryModel from '../../../models/category-model';
import { SearchBarComponent } from '@components/search-bar/search-bar.component';

@Component({
  selector: 'app-categories',
  imports: [MatButtonModule, MatTableModule, RouterLink, SearchBarComponent],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css',
})
export class CategoriesComponent {
  private route = inject(ActivatedRoute);
  categories = signal<CategoryModel[]>(this.route.snapshot.data['categories'] ?? []);
  columnsToDisplay = ['name', 'actions'];

  constructor() {
    this.route.data.subscribe((data) => {
      this.categories.set(data['categories'] as CategoryModel[]);
    });
  }

  edit(category: CategoryModel): void {
    console.log('edit', category);
  }

  delete(category: CategoryModel): void {
    console.log('delete', category);
  }
}
