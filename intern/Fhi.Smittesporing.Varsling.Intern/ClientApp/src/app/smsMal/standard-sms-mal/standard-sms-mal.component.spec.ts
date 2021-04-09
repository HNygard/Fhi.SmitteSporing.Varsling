import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StandardSmsMalComponent } from './standard-sms-mal.component';

describe('StandardSmsMalComponent', () => {
  let component: StandardSmsMalComponent;
  let fixture: ComponentFixture<StandardSmsMalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StandardSmsMalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StandardSmsMalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
