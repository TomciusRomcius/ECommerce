import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ItemSorter } from './item-sorter';

describe('ItemSorter', () => {
  let component: ItemSorter;
  let fixture: ComponentFixture<ItemSorter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ItemSorter]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ItemSorter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
