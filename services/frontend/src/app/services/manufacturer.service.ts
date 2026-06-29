import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import ApiResponse from '../models/api-response';
import CreateManufacturerRequest from '../models/create-manufacturer-request';
import CreateManufacturerResponse from '../models/create-manufacturer-response';
import ManufacturerModel from '../models/manufacturer-model';
import { unwrapApiResponse } from '../utils/unwrap-api-response';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ManufacturerService {
  private readonly http = inject(HttpClient);

  getManufacturers(): Observable<ManufacturerModel[]> {
    return unwrapApiResponse(
      this.http.get<ApiResponse<ManufacturerModel[]>>(`${environment.backendApi}/Manufacturer`),
    );
  }

  createManufacturer(request: CreateManufacturerRequest): Observable<CreateManufacturerResponse> {
    return this.http.post<CreateManufacturerResponse>(
      `${environment.backendApi}/Manufacturer`,
      request,
    );
  }
}
