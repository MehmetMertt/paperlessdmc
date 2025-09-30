import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentDetail } from './document-detail';

describe('DocumentDetail', () => {
  let component: DocumentDetail;
  let fixture: ComponentFixture<DocumentDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DocumentDetail]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentDetail);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
