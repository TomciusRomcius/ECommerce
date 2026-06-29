import { Component, inject, OnInit, signal } from '@angular/core';
import { ModalTemplateComponent } from '@components/modal-template/modal-template.component';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { StoreProductsService } from '../../../../../services/store-products.service';
import { ActivatedRoute, Router } from '@angular/router';

export interface EditStockDialogData {
  storeLocationId: number;
  productId: number;
  initialStock: number;
}

@Component({
  selector: 'app-edit-stock',
  imports: [
    ModalTemplateComponent,
    MatFormField,
    MatLabel,
    MatInput,
    ReactiveFormsModule,
    MatButton,
  ],
  templateUrl: './edit-stock.component.html',
})
export class EditStockComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly data = inject<EditStockDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<EditStockComponent>);
  private readonly storeProductsService = inject(StoreProductsService);

  loading = signal(false);
  error = signal('');

  form = new FormGroup({
    stock: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(10000)],
    }),
  });

  ngOnInit(): void {
    this.form.patchValue({
      stock: this.data.initialStock,
    });
  }

  submit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const { stock } = this.form.getRawValue();
    this.loading.set(true);
    this.error.set('');

    this.storeProductsService
      .updateStock({
        storeLocationId: this.data.storeLocationId,
        productId: this.data.productId,
        stock,
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close();
          void this.router.navigate([], {
            relativeTo: this.route,
            queryParamsHandling: 'preserve',
            onSameUrlNavigation: 'reload'
          });
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to update stock. You may need admin access.');
        },
      });
  }
}
