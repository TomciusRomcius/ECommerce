import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { PagePadder } from '../../components/page-padder/page-padder';
import { ACCESS_TOKEN_KEY } from '../../constants/auth';
import CartItemModel from '../../models/cart-item-model';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart-page',
  imports: [CurrencyPipe, MatButtonModule, MatCardModule, MatIconModule, PagePadder, RouterLink],
  templateUrl: './cart-page.html',
})
export class CartPage implements OnInit {
  private readonly router = inject(Router);
  private readonly cartService = inject(CartService);

  items = signal<CartItemModel[]>([]);
  error = signal<string | null>(null);
  loading = signal(true);
  removingKey = signal<string | null>(null);

  itemCount = computed(() =>
    this.items().reduce((total, item) => total + item.quantity, 0),
  );

  subtotal = computed(() =>
    this.items().reduce((total, item) => total + this.lineTotal(item), 0),
  );

  ngOnInit(): void {
    if (!sessionStorage.getItem(ACCESS_TOKEN_KEY)) {
      void this.router.navigate(['/login']);
      return;
    }

    this.loadItems();
  }

  reload(): void {
    this.error.set(null);
    this.loading.set(true);
    this.loadItems();
  }

  trackItem(_index: number, item: CartItemModel): string {
    return `${item.productId}-${item.storeLocationId}`;
  }

  lineTotal(item: CartItemModel): number {
    return (item.product?.price ?? 0) * item.quantity;
  }

  isRemoving(item: CartItemModel): boolean {
    return this.removingKey() === this.itemKey(item);
  }

  removeItem(item: CartItemModel): void {
    this.removingKey.set(this.itemKey(item));

    this.cartService.removeItem(item.productId, item.storeLocationId).subscribe({
      next: () => {
        this.items.update((items) =>
          items.filter((i) => !(i.productId === item.productId && i.storeLocationId === item.storeLocationId)),
        );
        this.removingKey.set(null);
      },
      error: () => {
        this.error.set('Failed to remove item from cart.');
        this.removingKey.set(null);
      },
    });
  }

  private loadItems(): void {
    this.cartService.getItems().subscribe({
      next: (items) => {
        this.items.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load cart items. Please try again.');
        this.loading.set(false);
      },
    });
  }

  private itemKey(item: CartItemModel): string {
    return `${item.productId}-${item.storeLocationId}`;
  }
}
