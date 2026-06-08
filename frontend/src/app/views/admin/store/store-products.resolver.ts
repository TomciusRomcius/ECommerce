import { HttpClient } from "@angular/common/http";
import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import ApiResponse from "@models/api-response";
import PageModel from "@models/page-model";
import ProductModel from "@models/product-model";
import { unwrapApiResponse } from "@utils/unwrap-api-response";
import { environment } from "../../../../environments/environment";

export const storeProductsResolver: ResolveFn<PageModel<ProductModel>> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const httpClient = inject(HttpClient);
  const storeLocationId = route.paramMap.get('id');
  const pageNumber = route.queryParamMap.get('pageNumber') ?? '1';
  const searchText = route.queryParamMap.get('searchText') ?? '';
  if (!storeLocationId) 
    throw new Error("Store location id is not defined!");
  const params: Record<string, string> = { storeLocationId, pageNumber, searchText };

  return unwrapApiResponse(
    httpClient.get<ApiResponse<PageModel<ProductModel>>>(`${environment.backendApi}/availableproducts`, {
      params,
    }));
}
