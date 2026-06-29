import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PaginatorComponent } from '../../../components/paginator/paginator.component';
import PageModel, { emptyPage } from '../../../models/page-model';
import ProductModel from '../../../models/product-model';
import { SearchBarComponent } from "@components/search-bar/search-bar.component";

@Component({
  selector: 'app-products',
  imports: [CurrencyPipe, MatButtonModule, MatTableModule, PaginatorComponent, RouterLink, SearchBarComponent],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css',
})
export class ProductsComponent {
  private route = inject(ActivatedRoute);
  products = signal<PageModel<ProductModel>>(
    this.route.snapshot.data['products'] ?? emptyPage(),
  );
  columnsToDisplay = ['name', 'price', 'actions'];

  constructor() {
    this.route.data.subscribe((data) => {
      this.products.set(data['products'] as PageModel<ProductModel>);
    });
  }

  edit(product: ProductModel): void {
    console.log('edit', product);
  }

  delete(product: ProductModel): void {
    console.log('delete', product);
  }
}
