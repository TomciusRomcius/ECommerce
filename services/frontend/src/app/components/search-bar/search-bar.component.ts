import { Component, inject, OnInit, signal } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { ActivatedRoute, Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, skip } from 'rxjs';

@Component({
  selector: 'app-search-bar',
  imports: [MatFormField, MatLabel, MatInput],
  templateUrl: './search-bar.component.html',
  styles: `
    :host {
      display: block;
      width: 100%;
    }
  `,
})
export class SearchBarComponent implements OnInit {
  text = signal('');
  textObs = toObservable(this.text).pipe(
    skip(1),
    debounceTime(500),
    distinctUntilChanged(),
  );
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    this.text.set(this.route.snapshot.queryParamMap.get('searchText') ?? '');

    this.textObs.subscribe((text) => {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { searchText: text || null, pageNumber: 1 },
        queryParamsHandling: 'merge',
      });
    });
  }

  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.text.set(input.value);
  }
}
