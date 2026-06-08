import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import PaymentSessionModel from '../models/payment-session.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);

  createPaymentSession(): Observable<PaymentSessionModel> {
    return this.http.post<PaymentSessionModel>(`${environment.backendApi}/orderflow/session`, {});
  }
}
