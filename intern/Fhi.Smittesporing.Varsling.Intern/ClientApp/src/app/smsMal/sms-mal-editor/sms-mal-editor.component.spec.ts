import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsMalEditorComponent } from './sms-mal-editor.component';

describe('SmsMalEditorComponent', () => {
  let component: SmsMalEditorComponent;
  let fixture: ComponentFixture<SmsMalEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsMalEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsMalEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
