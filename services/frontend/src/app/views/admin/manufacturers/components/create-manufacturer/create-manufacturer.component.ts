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
import { ManufacturerService } from '../../../../../services/manufacturer.service';

@Component({
  selector: 'app-create-manufacturer-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './create-manufacturer.component.html',
})
export class CreateManufacturerComponent {
  private readonly manufacturerService = inject(ManufacturerService);

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

    this.manufacturerService.createManufacturer({ name: this.form.controls.name.value }).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.successMessage.set(`Manufacturer created (id: ${response.manufacturerId}).`);
        this.form.reset();
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Failed to create manufacturer. You may need admin access.');
      },
    });
  }
}
