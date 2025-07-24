import { TestBed } from '@angular/core/testing';

import { GameFilterService } from './game-filter-service';

describe('GameFilterService', () => {
  let service: GameFilterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GameFilterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
