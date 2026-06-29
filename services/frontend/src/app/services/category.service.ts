import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import ApiResponse from '../models/api-response';
import CategoryModel from '../models/category-model';
import CreateCategoryRequest from '../models/create-category-request';
import CreateCategoryResponse from '../models/create-category-response';
import { unwrapApiResponse } from '../utils/unwrap-api-response';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly http = inject(HttpClient);

  getCategories(): Observable<CategoryModel[]> {
    return unwrapApiResponse(
      this.http.get<ApiResponse<CategoryModel[]>>(`${environment.backendApi}/Categories`),
    );
  }

  createCategory(request: CreateCategoryRequest): Observable<CreateCategoryResponse> {
    return this.http.post<CreateCategoryResponse>(
      `${environment.backendApi}/Categories`,
      request,
    );
  }
}
