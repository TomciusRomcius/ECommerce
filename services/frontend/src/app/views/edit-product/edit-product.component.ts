import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import CategoryModel from '../../models/category-model';
import ManufacturerModel from '../../models/manufacturer-model';
import ProductModel from '../../models/product-model';
import { CategoryService } from '../../services/category.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { ProductAdminService } from '../../services/product-admin.service';

@Component({
  selector: 'app-edit-product-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './edit-product.component.html',
})
export class EditProductComponent implements OnInit {
  private readonly productAdminService = inject(ProductAdminService);
  private readonly manufacturerService = inject(ManufacturerService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private productId = 0;

  loading = signal(false);
  optionsLoading = signal(true);
  error = signal('');
  manufacturers = signal<ManufacturerModel[]>([]);
  categories = signal<CategoryModel[]>([]);

  form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(5)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(5)],
    }),
    price: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0.1)],
    }),
    manufacturerId: new FormControl<number>(0, {
      nonNullable: true,
      validators: [Validators.required],
    }),
    categoryId: new FormControl<number>(0, {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  ngOnInit(): void {
    const product = this.route.snapshot.data['product'] as ProductModel;
    this.productId = product.productId;
    this.form.patchValue({
      name: product.name,
      description: product.description,
      price: product.price,
      manufacturerId: product.manufacturerId,
      categoryId: product.categoryId,
    });

    forkJoin({
      manufacturers: this.manufacturerService.getManufacturers(),
      categories: this.categoryService.getCategories(),
    }).subscribe({
      next: ({ manufacturers, categories }) => {
        this.manufacturers.set(manufacturers);
        this.categories.set(categories);
        this.optionsLoading.set(false);
      },
      error: () => {
        this.optionsLoading.set(false);
        this.error.set('Failed to load manufacturers or categories.');
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const { name, description, price, categoryId, manufacturerId } = this.form.getRawValue();

    this.productAdminService
      .updateProduct({
        productId: this.productId,
        name,
        description,
        price,
        manufacturerId,
        categoryId,
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          void this.router.navigate(['/admin/products']);
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to update product. You may need admin access.');
        },
      });
  }
}
