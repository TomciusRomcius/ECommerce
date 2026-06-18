import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { environment } from '../../../environments/environment';
import ApiResponse from '../../models/api-response';
import StoreLocationModel from '../../models/store-location-model';
import { unwrapApiResponse } from '../../utils/unwrap-api-response';
import PageModel from '@models/page-model';

export const storeLocationsResolver: ResolveFn<PageModel<StoreLocationModel>> = (
  route: ActivatedRouteSnapshot,
  _state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const page = route.queryParamMap.get('page') ?? '1';

  return unwrapApiResponse(
    httpClient.get<ApiResponse<PageModel<StoreLocationModel>>>(`${environment.backendApi}/storelocations`, {
      params: { page },
    }),
  );
};
