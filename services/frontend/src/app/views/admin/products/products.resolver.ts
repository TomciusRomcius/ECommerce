import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { environment } from '../../../../environments/environment';
import PageModel from '../../../models/page-model';
import ProductModel from '../../../models/product-model';

export const productsResolver: ResolveFn<PageModel<ProductModel>> = (
  route: ActivatedRouteSnapshot,
  _state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const pageNumber = route.queryParamMap.get('pageNumber') ?? '1';
  const pageSize = route.queryParamMap.get('pageSize') ?? '20';
  const searchText = route.queryParamMap.get('searchText') ?? '';

  return httpClient.get<PageModel<ProductModel>>(`${environment.backendApi}/Products`, {
    params: { pageNumber, pageSize, searchText },
  });
};
