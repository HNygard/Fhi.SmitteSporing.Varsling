import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VarslingsinfoComponent } from './varslingsinfo.component';

describe('VarslingsinfoComponent', () => {
  let component: VarslingsinfoComponent;
  let fixture: ComponentFixture<VarslingsinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VarslingsinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VarslingsinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
