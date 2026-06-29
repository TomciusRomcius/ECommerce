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
import { forkJoin } from 'rxjs';
import CategoryModel from '../../models/category-model';
import ManufacturerModel from '../../models/manufacturer-model';
import { CategoryService } from '../../services/category.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { ProductAdminService } from '../../services/product-admin.service';
import { UploadImagesComponent } from "../../components/upload-images/upload-images.component";

@Component({
  selector: 'app-create-product-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    UploadImagesComponent
],
  templateUrl: './create-product.component.html',
})
export class CreateProductComponent implements OnInit {
  private readonly productAdminService = inject(ProductAdminService);
  private readonly manufacturerService = inject(ManufacturerService);
  private readonly categoryService = inject(CategoryService);

  loading = signal(false);
  optionsLoading = signal(true);
  error = signal('');
  successMessage = signal<string | null>(null);
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
    manufacturerId: new FormControl<number | null>(null, Validators.required),
    categoryId: new FormControl<number | null>(null, Validators.required),
  });

  images = signal<File[]>([]);

  ngOnInit(): void {
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

    const manufacturerId = this.form.controls.manufacturerId.value;
    const categoryId = this.form.controls.categoryId.value;

    if (manufacturerId === null || categoryId === null) {
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.successMessage.set(null);

    const { name, description, price } = this.form.getRawValue();

    this.productAdminService
      .createProduct({ name, description, price, manufacturerId, categoryId, files: this.images() })
      .subscribe({
        next: (response) => {
          this.loading.set(false);
          this.successMessage.set(`Product created (id: ${response.productId}).`);
          this.images.set([]);
          this.form.reset({
            name: '',
            description: '',
            price: 0,
            manufacturerId: null,
            categoryId: null,
          });
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to create product. You may need admin access.');
        },
      });
  }
}
