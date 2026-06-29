import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import CreateStoreLocationRequest from '../models/create-store-location-request';
import CreateStoreLocationResponse from '../models/create-store-location-response';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class StoreLocationService {
  private readonly http = inject(HttpClient);

  createStoreLocation(
    request: CreateStoreLocationRequest,
  ): Observable<CreateStoreLocationResponse> {
    return this.http.post<CreateStoreLocationResponse>(
      `${environment.backendApi}/StoreLocations`,
      request,
    );
  }
}
