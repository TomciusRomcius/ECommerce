import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { PagePadder } from "../../components/page-padder/page-padder";
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import ProductModel from '@models/product-model';
import { Paginator } from "@components/paginator/paginator";
import PageModel, { emptyPage } from '@models/page-model';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-home',
  imports: [CurrencyPipe, MatButtonModule, MatCardModule, MatInputModule, ReactiveFormsModule, FormsModule, MatFormFieldModule, MatSelectModule, MatIconModule, PagePadder, RouterLink, Paginator],
  templateUrl: './store-page.html',
})
export class StorePage {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);

  products = signal<PageModel<ProductModel>>(emptyPage());
  storeLocationId = signal<number | null>(null);
  orderBy = signal<string>('name');
  orderType = signal<string>('asc');

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
      this.orderBy.set(params.get('orderBy') ?? 'name');
      this.orderType.set(params.get('orderType') ?? 'asc');
    });

    this.activatedRoute.data.subscribe(({ products }) => {
      this.products.set(products);
    });
  }

  onSelectOrderBy(value: string): void {
    this.router.navigate([], {
      queryParams: { orderBy: value, pageNumber: 1 },
      queryParamsHandling: 'merge',
    });
  }

  onSelectOrderType(value: string): void {
    this.router.navigate([], {
      queryParams: { orderType: value, pageNumber: 1 },
      queryParamsHandling: 'merge',
    });
  }
}
