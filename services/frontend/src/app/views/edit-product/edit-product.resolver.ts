import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import ProductModel from '@models/product-model';
import { environment } from '@environment';

export const editProductResolver: ResolveFn<ProductModel> = (
  route: ActivatedRouteSnapshot,
  _state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const productId = route.paramMap.get('productId');

  return httpClient.get<ProductModel>(`${environment.backendApi}/products/${productId}`);
};
