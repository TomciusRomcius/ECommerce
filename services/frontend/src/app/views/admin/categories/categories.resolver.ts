import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { environment } from '../../../../environments/environment';
import ApiResponse from '../../../models/api-response';
import CategoryModel from '../../../models/category-model';
import { unwrapApiResponse } from '../../../utils/unwrap-api-response';

export const categoriesResolver: ResolveFn<CategoryModel[]> = (
  route: ActivatedRouteSnapshot,
  _state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const searchText = route.queryParamMap.get('searchText') ?? '';

  return unwrapApiResponse(
    httpClient.get<ApiResponse<CategoryModel[]>>(`${environment.backendApi}/Categories`, {
      params: { searchText },
    }),
  );
};
