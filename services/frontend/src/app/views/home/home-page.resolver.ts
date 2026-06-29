import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import ProductModel from "../../models/product-model";
import ApiResponse from "../../models/api-response";
import PageModel from "../../models/page-model";
import { inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";
import { unwrapApiResponse } from "../../utils/unwrap-api-response";

export const productResolver: ResolveFn<PageModel<ProductModel>> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const storeLocationId = route.queryParamMap.get('storeLocationId');
  const pageNumber = route.queryParamMap.get('pageNumber') ?? '1';
  const pageSize = route.queryParamMap.get('pageSize') ?? '20';

  const params: Record<string, string> = { pageNumber, pageSize };
  if (storeLocationId) {
    params['storeLocationId'] = storeLocationId;
  }

  params['orderBy'] = route.queryParamMap.get('orderBy') ?? 'name';
  params['orderType'] = route.queryParamMap.get('orderType') ?? 'asc';

  return unwrapApiResponse(
    httpClient.get<ApiResponse<PageModel<ProductModel>>>(`${environment.backendApi}/storeproducts`, {
      params,
    }));
}
