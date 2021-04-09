import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndekspasienterComponent } from './indekspasienter.component';

describe('IndekspasienterComponent', () => {
  let component: IndekspasienterComponent;
  let fixture: ComponentFixture<IndekspasienterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IndekspasienterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IndekspasienterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
