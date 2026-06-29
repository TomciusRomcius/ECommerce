import { map, Observable } from 'rxjs';
import ApiResponse from '../models/api-response';

export function unwrapApiResponse<T>(source: Observable<ApiResponse<T>>): Observable<T> {
  return source.pipe(map((response) => response.data));
}
