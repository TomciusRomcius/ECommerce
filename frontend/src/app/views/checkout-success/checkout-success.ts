import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { MatCard } from "@angular/material/card";

@Component({
  selector: 'app-checkout-success',
  imports: [MatCard],
  templateUrl: './checkout-success.html',
})
export class CheckoutSuccessPage {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private http = inject(HttpClient);

  constructor() {
    this.route.queryParamMap.subscribe((params) => {
      const sessionId = params.get('session_id');
      if (!sessionId)
      {
        this.router.navigate(['/']);
        return;
      }
      this.verifyPayment(sessionId);
    });    
  }

  private verifyPayment(sessionId: string) {
    this.http.post(`${environment.backendApi}/checkout/verify`, {
      sessionId
    }).subscribe(() => this.router.navigate(['/']));
  }
}
