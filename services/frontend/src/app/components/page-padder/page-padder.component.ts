import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-page-padder',
  imports: [MatIconModule, RouterLink, RouterLinkActive],
  templateUrl: './page-padder.component.html',
})
export class PagePadderComponent {}
