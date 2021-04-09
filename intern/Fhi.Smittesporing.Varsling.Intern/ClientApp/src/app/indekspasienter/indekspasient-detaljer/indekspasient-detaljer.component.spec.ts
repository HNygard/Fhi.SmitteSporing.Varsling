import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndekspasientDetaljerComponent } from './indekspasient-detaljer.component';

describe('IndekspasientDetaljerComponent', () => {
  let component: IndekspasientDetaljerComponent;
  let fixture: ComponentFixture<IndekspasientDetaljerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IndekspasientDetaljerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IndekspasientDetaljerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
