import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Paginator } from '../../../components/paginator/paginator';
import PageModel, { emptyPage } from '../../../models/page-model';
import ProductModel from '../../../models/product-model';
import StoreLocationModel from '../../../models/store-location-model';
import { MatDialog } from '@angular/material/dialog';
import { EditStock } from './components/edit-stock/edit-stock';
import { AddProductToStore } from './components/add-product-to-store/add-product-to-store';
import { SearchBar } from "@components/search-bar/search-bar";

@Component({
  selector: 'app-store',
  imports: [CurrencyPipe, MatButtonModule, MatTableModule, Paginator, RouterLink, SearchBar],
  templateUrl: './store.html',
  styleUrl: './store.css',
})
export class Store {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  storeLocation = signal<StoreLocationModel | null>(
    this.route.snapshot.data['storeLocation'] ?? null,
  );
  products = signal<PageModel<ProductModel>>(
    this.route.snapshot.data['products'] ?? emptyPage(),
  );
  columnsToDisplay = ['name', 'price', 'stock', 'actions'];
  private dialog = inject(MatDialog);

  constructor() {
    this.route.data.subscribe((data) => {
      this.storeLocation.set(data['storeLocation'] as StoreLocationModel);
      this.products.set(data['products'] as PageModel<ProductModel>);
    });
  }

  private reloadProducts(): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParamsHandling: 'preserve',
      onSameUrlNavigation: 'reload'
    });
  }

  editStock(product: ProductModel): void {
    const storeLocationId = this.storeLocation()?.storeLocationId;
    if (!storeLocationId) {
      return;
    }

    this.dialog.open(EditStock, {
      data: {
        storeLocationId,
        productId: product.productId,
        initialStock: product.stock ?? 0,
      },
    });
  }

  addProduct(): void {
    const storeLocationId = this.storeLocation()?.storeLocationId;
    if (!storeLocationId) {
      return;
    }

    this.dialog.open(AddProductToStore, {
      data: {
        storeLocationId,
      },
      minWidth: '700px',
    });
  }

  delete(product: ProductModel): void {
    console.log('delete', product);
  }
}
