import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Searchbar } from './searchbar';

describe('Searchbar', () => {
  let component: Searchbar;
  let fixture: ComponentFixture<Searchbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Searchbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Searchbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
