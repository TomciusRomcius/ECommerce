import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import ApiResponse from '../../models/api-response';
import ProductModel from '../../models/product-model';
import { unwrapApiResponse } from '../../utils/unwrap-api-response';
import { environment } from '../../../environments/environment';

export const productDetailResolver: ResolveFn<ProductModel> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const productId = route.paramMap.get('productId');

  return unwrapApiResponse(
    httpClient.get<ApiResponse<ProductModel>>(`${environment.backendApi}/storeproducts/${productId}`),
  );
};
