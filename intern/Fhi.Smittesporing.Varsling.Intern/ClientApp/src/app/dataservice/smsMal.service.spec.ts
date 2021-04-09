import { TestBed } from '@angular/core/testing';

import { SmsMalService } from './smsMal.service';

describe('SmsMalService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SmsMalService = TestBed.get(SmsMalService);
    expect(service).toBeTruthy();
  });
});
