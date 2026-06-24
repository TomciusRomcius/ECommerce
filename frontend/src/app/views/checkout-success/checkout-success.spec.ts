import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutSuccessPage } from './checkout-success';

describe('CheckoutSuccess', () => {
  let component: CheckoutSuccessPage;
  let fixture: ComponentFixture<CheckoutSuccessPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckoutSuccessPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutSuccessPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
