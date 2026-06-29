import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { PagePadderComponent } from './page-padder.component';

describe('PagePadderComponent', () => {
  let component: PagePadderComponent;
  let fixture: ComponentFixture<PagePadderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PagePadderComponent],
      providers: [provideRouter([])],
    })
    .compileComponents();

    fixture = TestBed.createComponent(PagePadderComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
