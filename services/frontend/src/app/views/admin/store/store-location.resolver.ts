import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import ApiResponse from '@models/api-response';
import StoreLocationModel from '@models/store-location-model';
import { unwrapApiResponse } from '@utils/unwrap-api-response';
import { environment } from '../../../../environments/environment';

export const storeLocationResolver: ResolveFn<StoreLocationModel> = (
  route: ActivatedRouteSnapshot,
  _state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const id = route.paramMap.get('id');
  if (!id) {
    throw new Error('Store location id is not defined!');
  }

  return unwrapApiResponse(
    httpClient.get<ApiResponse<StoreLocationModel>>(
      `${environment.backendApi}/storelocations/${id}`,
    ),
  );
};
