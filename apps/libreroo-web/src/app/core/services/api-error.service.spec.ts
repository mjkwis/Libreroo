import { HttpErrorResponse } from '@angular/common/http';
import { ApiErrorService } from './api-error.service';

describe('ApiErrorService', () => {
  let service: ApiErrorService;

  beforeEach(() => {
    service = new ApiErrorService();
  });

  it('should surface backend domain error text when present', () => {
    const error = new HttpErrorResponse({ status: 409, error: { error: 'No available copies.' } });
    expect(service.map(error)).toBe('No available copies.');
  });

  it('should fallback to generic message for network or unexpected failures', () => {
    const error = new HttpErrorResponse({ status: 0, error: null });
    expect(service.map(error)).toBe('Request failed. Please retry.');
  });
});
