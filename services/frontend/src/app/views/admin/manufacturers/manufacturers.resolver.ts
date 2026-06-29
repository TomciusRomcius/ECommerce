import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import ManufacturerModel from "../../../models/manufacturer-model";
import { HttpClient } from "@angular/common/http";
import { inject } from "@angular/core";
import { environment } from "../../../../environments/environment";
import ApiResponse from "../../../models/api-response";
import { unwrapApiResponse } from "../../../utils/unwrap-api-response";

export const manufacturersResolver: ResolveFn<ManufacturerModel[]> = (
    route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot,
  ) => {
    const httpClient = inject(HttpClient);
    const page = route.queryParamMap.get('page') ?? '1';
    const searchText = route.queryParamMap.get('searchText') ?? '';
  
    return unwrapApiResponse(
      httpClient.get<ApiResponse<ManufacturerModel[]>>(`${environment.backendApi}/manufacturer`, {
        params: { page, searchText },
      }),
    );
  };
  