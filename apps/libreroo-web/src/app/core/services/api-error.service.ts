import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  map(error: unknown): string {
    if (error instanceof HttpErrorResponse && this.hasBackendMessage(error.error)) {
      return error.error.error;
    }

    return 'Request failed. Please retry.';
  }

  private hasBackendMessage(value: unknown): value is { error: string } {
    return (
      typeof value === 'object' &&
      value !== null &&
      'error' in value &&
      typeof value.error === 'string' &&
      value.error.trim().length > 0
    );
  }
}
