import { CurrencyPipe } from '@angular/common';
import { Component, effect, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientPaginatorComponent } from '@components/client-paginator/client-paginator.component';
import ProductModel from '@models/product-model';
import { emptyPage } from '@models/page-model';
import { ModalTemplateComponent } from '../../../../../components/modal-template/modal-template.component';
import { StoreProductsService } from '../../../../../services/store-products.service';

export interface AddProductToStoreDialogData {
  storeLocationId: number;
}

@Component({
  selector: 'app-add-product-to-store',
  imports: [
    ModalTemplateComponent,
    MatButton,
    MatTableModule,
    CurrencyPipe,
    ClientPaginatorComponent,
    MatFormField,
    MatLabel,
    MatInput,
    ReactiveFormsModule,
  ],
  templateUrl: './add-product-to-store.component.html',
  styleUrl: './add-product-to-store.component.css',
})
export class AddProductToStoreComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly data = inject<AddProductToStoreDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<AddProductToStoreComponent>);
  private readonly storeProductsService = inject(StoreProductsService);
  readonly pageSize = 10;
  currentPage = signal(1);

  products = signal(emptyPage<ProductModel>());
  loadingProducts = signal(false);
  loadingSubmit = signal(false);
  error = signal('');
  selectedProduct = signal<ProductModel | null>(null);
  columnsToDisplay = ['name', 'price', 'actions'];

  form = new FormGroup({
    stock: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(10000)],
    }),
  });

  constructor() {
    effect(() => {
      this.loadProducts(this.currentPage());
    });
  }

  onPageChange(pageNumber: number): void {
    this.currentPage.set(pageNumber);
  }

  selectProduct(product: ProductModel): void {
    this.selectedProduct.set(product);
    this.error.set('');
  }

  submit(): void {
    if (!this.selectedProduct()) {
      this.error.set('Select a product first.');
      return;
    }

    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const { stock } = this.form.getRawValue();
    const selectedProduct = this.selectedProduct();
    if (!selectedProduct) {
      return;
    }

    this.loadingSubmit.set(true);
    this.error.set('');

    this.storeProductsService
      .addProductToStore({
        storeLocationId: this.data.storeLocationId,
        productId: selectedProduct.productId,
        stock,
      })
      .subscribe({
        next: () => {
          this.loadingSubmit.set(false);
          this.dialogRef.close();
          void this.router.navigate([], {
            relativeTo: this.route,
            queryParamsHandling: 'preserve',
            onSameUrlNavigation: 'reload',
          });
        },
        error: () => {
          this.loadingSubmit.set(false);
          this.error.set('Failed to add product to store. You may need admin access.');
        },
      });
  }

  private loadProducts(pageNumber: number): void {
    this.loadingProducts.set(true);
    this.storeProductsService.getProducts(pageNumber, this.pageSize).subscribe({
      next: (products) => {
        this.products.set(products);
        this.loadingProducts.set(false);
      },
      error: () => {
        this.loadingProducts.set(false);
        this.error.set('Failed to load products.');
      },
    });
  }

}
