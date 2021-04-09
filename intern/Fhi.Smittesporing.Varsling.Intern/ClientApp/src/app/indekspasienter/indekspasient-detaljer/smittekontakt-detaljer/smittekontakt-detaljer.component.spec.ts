import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmittekontaktDetaljerComponent } from './smittekontakt-detaljer.component';

describe('SmittekontaktDetaljerComponent', () => {
  let component: SmittekontaktDetaljerComponent;
  let fixture: ComponentFixture<SmittekontaktDetaljerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmittekontaktDetaljerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmittekontaktDetaljerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
