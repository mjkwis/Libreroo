import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from './core/api-base-url.token';
import { appConfig } from './app.config';

describe('app config', () => {
  it('should provide configured api base url', () => {
    TestBed.configureTestingModule({
      providers: appConfig.providers
    });

    expect(TestBed.inject(API_BASE_URL)).toBe('');
  });
});
