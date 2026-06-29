import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadImagesComponent } from './upload-images.component';

describe('UploadImagesComponent', () => {
  let component: UploadImagesComponent;
  let fixture: ComponentFixture<UploadImagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UploadImagesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UploadImagesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
