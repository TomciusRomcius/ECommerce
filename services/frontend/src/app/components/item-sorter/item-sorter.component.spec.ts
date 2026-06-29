import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ItemSorterComponent } from './item-sorter.component';

describe('ItemSorterComponent', () => {
  let component: ItemSorterComponent;
  let fixture: ComponentFixture<ItemSorterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ItemSorterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ItemSorterComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
