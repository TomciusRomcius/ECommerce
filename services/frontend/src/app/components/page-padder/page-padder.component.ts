import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-page-padder',
  imports: [MatIconModule, MatMenuModule, RouterLink, RouterLinkActive],
  templateUrl: './page-padder.component.html',
})
export class PagePadderComponent {
  private readonly auth = inject(AuthService);

  isLoggedIn(): boolean {
    return this.auth.isLoggedIn();
  }

  logout(): void {
    this.auth.logout();
  }
}
