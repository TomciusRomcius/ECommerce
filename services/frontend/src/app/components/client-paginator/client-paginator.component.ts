import { Component, computed, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-client-paginator',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './client-paginator.component.html',
  styleUrl: './client-paginator.component.css',
})
export class ClientPaginatorComponent {
  private static readonly maxVisiblePages = 6;

  totalCount = input.required<number>();
  pageSize = input.required<number>();
  pageNumber = input.required<number>();
  pageNumberChange = output<number>();

  totalPages = computed(() => {
    const pageSize = this.pageSize();
    if (!pageSize) {
      return 0;
    }
    return Math.ceil(this.totalCount() / pageSize);
  });

  showArrows = computed(() => this.totalPages() > ClientPaginatorComponent.maxVisiblePages);

  visiblePages = computed(() => {
    const total = this.totalPages();
    if (total === 0) {
      return [];
    }
    if (total <= ClientPaginatorComponent.maxVisiblePages) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    const current = this.pageNumber();
    const start = Math.max(
      1,
      Math.min(
        current - Math.floor(ClientPaginatorComponent.maxVisiblePages / 2),
        total - ClientPaginatorComponent.maxVisiblePages + 1,
      ),
    );
    return Array.from({ length: ClientPaginatorComponent.maxVisiblePages }, (_, i) => start + i);
  });

  canGoPrev = computed(() => this.pageNumber() > 1);
  canGoNext = computed(() => this.pageNumber() < this.totalPages());

  isCurrentPage(page: number): boolean {
    return page === this.pageNumber();
  }

  goToPage(page: number): void {
    if (page !== this.pageNumber()) {
      this.pageNumberChange.emit(page);
    }
  }

  goToPrev(): void {
    this.goToPage(this.pageNumber() - 1);
  }

  goToNext(): void {
    this.goToPage(this.pageNumber() + 1);
  }
}
