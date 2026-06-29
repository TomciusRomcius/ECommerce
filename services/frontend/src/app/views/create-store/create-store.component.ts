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
import { StoreLocationService } from '../../services/store-location.service';

@Component({
  selector: 'app-create-store-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './create-store.component.html',
})
export class CreateStoreComponent {
  private readonly storeLocationService = inject(StoreLocationService);

  loading = signal(false);
  error = signal('');
  successMessage = signal<string | null>(null);

  form = new FormGroup({
    displayName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100),
      ],
    }),
    address: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(200),
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

    this.storeLocationService
      .createStoreLocation({
        displayName: this.form.controls.displayName.value,
        address: this.form.controls.address.value,
      })
      .subscribe({
        next: (response) => {
          this.loading.set(false);
          this.successMessage.set(`Store created (id: ${response.storeLocationId}).`);
          this.form.reset();
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to create store. You need admin access.');
        },
      });
  }
}
