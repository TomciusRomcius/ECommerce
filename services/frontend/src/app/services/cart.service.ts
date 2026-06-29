import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import ApiResponse from '../models/api-response';
import AddCartItemRequest from '../models/add-cart-item-request';
import CartItemModel from '../models/cart-item-model';
import { unwrapApiResponse } from '../utils/unwrap-api-response';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);

  getItems(): Observable<CartItemModel[]> {
    return unwrapApiResponse(
      this.http.get<ApiResponse<CartItemModel[]>>(`${environment.backendApi}/cart`),
    );
  }

  addItem(request: AddCartItemRequest): Observable<void> {
    return this.http.post<void>(`${environment.backendApi}/cart`, request);
  }

  removeItem(productId: number, storeLocationId: number): Observable<void> {
    return this.http.delete<void>(`${environment.backendApi}/cart`, {
      params: { productId, storeLocationId },
    });
  }
}
