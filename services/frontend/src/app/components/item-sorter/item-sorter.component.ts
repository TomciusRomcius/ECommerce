import { Component, inject, input, model, OnInit, signal } from '@angular/core';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatSelect, MatOption } from '@angular/material/select';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

export interface OrderColumn {
  displayText: string;
  apiColumn: string;
}
export type OrderType = 'asc' | 'desc';

function isOrderType(rawOrderType: string) {
  return rawOrderType === 'asc' || rawOrderType === 'desc';
}

@Component({
  selector: 'app-item-sorter',
  imports: [MatFormField, MatLabel, MatIcon, MatSelect, MatOption],
  templateUrl: './item-sorter.component.html',
})
export class ItemSorterComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  fields = input.required<OrderColumn[]>();
  defaultOrderColumn = input.required<OrderColumn>();
  defaultOrderType = input.required<OrderType>();

  orderBy = signal<OrderColumn>({} as OrderColumn);
  orderType = signal<OrderType>({} as OrderType);

  ngOnInit(): void {
    this.orderBy.set(this.defaultOrderColumn());
    this.route.queryParamMap.subscribe((params) => {
      this.computeOrderByFromParams(params);
      this.computeOrderTypeFromParams(params);
    });
  }

  setOrderBy(value: OrderColumn) {
    this.router.navigate([], {
      queryParams: { orderBy: value.apiColumn, pageNumber: 1 },
      queryParamsHandling: 'merge',
    });
  }

  setOrderType(value: OrderType) {
    this.router.navigate([], {
      queryParams: { orderType: value, pageNumber: 1 },
      queryParamsHandling: 'merge',
    });
  }

  private computeOrderByFromParams(params: ParamMap) {
    const rawOrderBy = params.get('orderBy') ?? '';
    const orderBy = this.fields().find(
      (f) => f.apiColumn.toLowerCase() === rawOrderBy.toLowerCase(),
    );
    this.orderBy.set(orderBy ?? this.defaultOrderColumn());
  }

  private computeOrderTypeFromParams(params: ParamMap) {
    const rawOrderType = params.get('orderType') ?? this.defaultOrderType();
    const orderType: OrderType = isOrderType(rawOrderType)
      ? rawOrderType
      : this.defaultOrderType();
    this.orderType.set(orderType);
  }
}
