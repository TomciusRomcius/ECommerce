import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { PagePadderComponent } from '../../components/page-padder/page-padder.component';
import { ACCESS_TOKEN_KEY } from '../../constants/auth';

@Component({
  selector: 'app-admin-page',
  imports: [PagePadderComponent, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin.component.html',
})
export class AdminComponent implements OnInit {
  private readonly router = inject(Router);

  ngOnInit(): void {
    if (!sessionStorage.getItem(ACCESS_TOKEN_KEY)) {
      void this.router.navigate(['/login']);
    }
  }
}
