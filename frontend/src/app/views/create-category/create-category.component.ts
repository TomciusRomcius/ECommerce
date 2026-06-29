import { Component, inject, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-create-category-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './create-category.component.html',
})
export class CreateCategoryComponent {
  private readonly categoryService = inject(CategoryService);

  loading = signal(false);
  error = signal('');
  successMessage = signal<string | null>(null);

  form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(20),
      ],
    }),
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.successMessage.set(null);

    this.categoryService.createCategory({ name: this.form.controls.name.value }).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.successMessage.set(`Category created (id: ${response.categoryId}).`);
        this.form.reset();
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Failed to create category. You need admin access.');
      },
    });
  }
}
