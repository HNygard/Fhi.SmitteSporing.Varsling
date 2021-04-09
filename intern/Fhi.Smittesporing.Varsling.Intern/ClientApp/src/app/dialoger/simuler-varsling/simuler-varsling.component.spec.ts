import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SimulerVarslingComponent } from './simuler-varsling.component';

describe('SimulerVarslingComponent', () => {
  let component: SimulerVarslingComponent;
  let fixture: ComponentFixture<SimulerVarslingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SimulerVarslingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SimulerVarslingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
