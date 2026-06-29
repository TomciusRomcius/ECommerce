import { Component, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import PageModel from '@models/page-model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';
import { map } from 'rxjs';

@Component({
  selector: 'app-paginator',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './paginator.component.html',
  styleUrl: './paginator.component.css',
})
export class PaginatorComponent {
  private static readonly maxVisiblePages = 6;

  data = input.required<PageModel<any>>();
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  currentPage = toSignal(
    this.route.queryParamMap.pipe(map((params) => PaginatorComponent.parsePageNumber(params.get('pageNumber')))),
    { initialValue: PaginatorComponent.parsePageNumber(this.route.snapshot.queryParamMap.get('pageNumber')) },
  );

  totalPages = computed(() => {
    const { totalCount, pageSize } = this.data();
    if (!pageSize) {
      return 0;
    }
    return Math.ceil(totalCount / pageSize);
  });

  showArrows = computed(() => this.totalPages() > PaginatorComponent.maxVisiblePages);

  visiblePages = computed(() => {
    const total = this.totalPages();
    if (total === 0) {
      return [];
    }
    if (total <= PaginatorComponent.maxVisiblePages) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    const current = this.currentPage();
    const start = Math.max(
      1,
      Math.min(current - Math.floor(PaginatorComponent.maxVisiblePages / 2), total - PaginatorComponent.maxVisiblePages + 1),
    );
    return Array.from({ length: PaginatorComponent.maxVisiblePages }, (_, i) => start + i);
  });

  canGoPrev = computed(() => this.currentPage() > 1);
  canGoNext = computed(() => this.currentPage() < this.totalPages());

  isCurrentPage(page: number): boolean {
    return page === this.currentPage();
  }

  goToPage(page: number): void {
    this.router.navigate([], {
      queryParams: { pageNumber: page },
      queryParamsHandling: 'merge',
    });
  }

  goToPrev(): void {
    this.goToPage(this.currentPage() - 1);
  }

  goToNext(): void {
    this.goToPage(this.currentPage() + 1);
  }

  private static parsePageNumber(raw: string | null): number {
    const n = Number(raw ?? '1');
    return Number.isFinite(n) && n >= 1 ? Math.floor(n) : 1;
  }
}
