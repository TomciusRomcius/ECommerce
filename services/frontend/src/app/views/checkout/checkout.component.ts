import { isPlatformBrowser } from '@angular/common';
import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PagePadderComponent } from '../../components/page-padder/page-padder.component';
import { ACCESS_TOKEN_KEY } from '../../constants/auth';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-checkout-page',
  imports: [MatButtonModule, PagePadderComponent, RouterLink],
  templateUrl: './checkout.component.html',
})
export class CheckoutComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly orderService = inject(OrderService);
  private readonly platformId = inject(PLATFORM_ID);

  loading = signal(false);
  error = signal<string | null>(null);
  paymentStatus = signal<'success' | 'cancelled' | null>(null);

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    if (!sessionStorage.getItem(ACCESS_TOKEN_KEY)) {
      void this.router.navigate(['/login']);
      return;
    }

    const payment = this.route.snapshot.queryParamMap.get('payment');
    if (payment === 'success') {
      this.paymentStatus.set('success');
      return;
    }

    if (payment === 'cancelled') {
      this.paymentStatus.set('cancelled');
      return;
    }

    this.startCheckout();
  }

  retryCheckout(): void {
    this.error.set(null);
    this.paymentStatus.set(null);
    this.startCheckout();
  }

  private startCheckout(): void {
    this.loading.set(true);
    this.error.set(null);

    this.orderService.createPaymentSession().subscribe({
      next: (session) => {
        if (!session.checkoutUrl) {
          this.loading.set(false);
          this.error.set('No checkout URL returned from the server.');
          return;
        }

        window.location.href = session.checkoutUrl;
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Failed to create the payment session.');
      },
    });
  }
}
