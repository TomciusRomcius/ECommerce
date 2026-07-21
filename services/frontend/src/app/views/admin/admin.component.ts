import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { PagePadderComponent } from '../../components/page-padder/page-padder.component';
import { AuthService } from '../../services/auth.service';
import { ADMIN_ROLE } from '../../constants/auth';

@Component({
  selector: 'app-admin-page',
  imports: [PagePadderComponent, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin.component.html',
})
export class AdminComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);

  ngOnInit(): void {
    if (!this.auth.isLoggedIn()) {
      void this.router.navigate(['/login']);
      return;
    }

    this.auth.getUser().subscribe({
      next: (user) => {
        if (!user.roles.includes(ADMIN_ROLE)) {
          void this.router.navigate(['/home']);
        }
      },
      error: () => {
        void this.router.navigate(['/home']);
      },
    });
  }
}
