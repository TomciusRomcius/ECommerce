import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import CreateProductRequest from '../models/create-product-request';
import CreateProductResponse from '../models/create-product-response';
import UpdateProductRequest from '../models/update-product-request';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductAdminService {
  private readonly http = inject(HttpClient);

  createProduct(request: CreateProductRequest): Observable<CreateProductResponse> {
    const formData = new FormData();
    request.files.forEach((file) => formData.append('files', file));

    formData.append('name', request.name);
    formData.append('description', request.description);
    formData.append('price', String(request.price));
    formData.append('manufacturerId', String(request.manufacturerId));
    formData.append('categoryId', String(request.categoryId));

    return this.http.post<CreateProductResponse>(
      `${environment.backendApi}/Products`,
      formData
    );
  }

  updateProduct(request: UpdateProductRequest): Observable<void> {
    return this.http.post<void>(
      `${environment.backendApi}/Products/${request.productId}`,
      {
        name: request.name,
        description: request.description,
        price: request.price,
        manufacturerId: request.manufacturerId,
        categoryId: request.categoryId,
      },
    );
  }
}
