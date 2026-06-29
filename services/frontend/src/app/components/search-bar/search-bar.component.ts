import { Component, inject, OnInit, signal } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { ActivatedRoute, Router } from '@angular/router';
import { debounceTime } from 'rxjs';

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
  textObs = toObservable(this.text).pipe(debounceTime(500));
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  
  ngOnInit(): void {
    this.textObs.subscribe((text) => {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { searchText: text }
      })
    })
  }

  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.text.set(input.value);
  }
}
