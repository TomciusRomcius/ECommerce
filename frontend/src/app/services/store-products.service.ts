import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import PageModel from '../models/page-model';
import ProductModel from '../models/product-model';
import ApiResponse from '../models/api-response';
import { unwrapApiResponse } from '../utils/unwrap-api-response';

export interface UpdateProductStockRequest {
  storeLocationId: number;
  productId: number;
  stock: number;
}

@Injectable({ providedIn: 'root' })
export class StoreProductsService {
  private readonly http = inject(HttpClient);

  getProducts(pageNumber: number, pageSize = 10): Observable<PageModel<ProductModel>> {
    return unwrapApiResponse(
      this.http.get<ApiResponse<PageModel<ProductModel>>>(`${environment.backendApi}/availableproducts`, {
        params: {
          pageNumber,
          pageSize,
        },
      }),
    );
  }

  updateStock(request: UpdateProductStockRequest): Observable<void> {
    return this.http.put<void>(`${environment.backendApi}/availableproducts`, request);
  }

  addProductToStore(request: UpdateProductStockRequest): Observable<void> {
    return this.http.post<void>(`${environment.backendApi}/availableproducts`, request);
  }
}
