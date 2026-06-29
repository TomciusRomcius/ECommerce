import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { PagePadderComponent } from "../../components/page-padder/page-padder.component";
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import ProductModel from '@models/product-model';
import { PaginatorComponent } from "@components/paginator/paginator.component";
import PageModel, { emptyPage } from '@models/page-model';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { ItemSorterComponent, OrderColumn, OrderType } from "@components/item-sorter/item-sorter.component";

@Component({
  selector: 'app-home',
  imports: [CurrencyPipe, MatButtonModule, MatCardModule, MatInputModule, ReactiveFormsModule, FormsModule, MatFormFieldModule, MatSelectModule, MatIconModule, PagePadderComponent, RouterLink, PaginatorComponent, ItemSorterComponent],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);

  products = signal<PageModel<ProductModel>>(emptyPage());
  storeLocationId = signal<number | null>(null);
  sortingFields: OrderColumn[] = [
    { apiColumn: 'price', displayText: 'Price' },
    { apiColumn: 'name', displayText: 'Name' },
    { apiColumn: 'stock', displayText: 'Stock' },
    { apiColumn: 'storelocation', displayText: 'Store Location' }
  ];
  defaultSortingColumn = this.sortingFields[0];
  defaultOrderType: OrderType = 'asc';

  isStoreFiltered = computed(() => this.storeLocationId() !== null);

  storeHeading = computed(() => {
    if (!this.isStoreFiltered()) {
      return null;
    }

    return this.products().data[0]?.store?.displayName ?? `Store #${this.storeLocationId()}`;
  });

  storeAddress = computed(() => {
    if (!this.isStoreFiltered()) {
      return null;
    }

    return this.products().data[0]?.store?.address ?? null;
  });

  constructor() {
    this.activatedRoute.queryParamMap.subscribe((params) => {
      const id = params.get('storeLocationId');
      this.storeLocationId.set(id ? Number(id) : null);
    });

    this.activatedRoute.data.subscribe(({ products }) => {
      this.products.set(products);
    });
  }
}
