import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BekreftComponent } from './bekreft.component';

describe('BekreftComponent', () => {
  let component: BekreftComponent;
  let fixture: ComponentFixture<BekreftComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BekreftComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BekreftComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
