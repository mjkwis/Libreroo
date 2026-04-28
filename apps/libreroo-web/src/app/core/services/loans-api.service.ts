import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url.token';
import { BorrowBookRequest, Loan } from '../models';

@Injectable({ providedIn: 'root' })
export class LoansApiService {
  constructor(
    private readonly http: HttpClient,
    @Inject(API_BASE_URL) private readonly baseUrl: string
  ) {}

  borrow(request: BorrowBookRequest): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.resolveUrl('/loans'), request);
  }

  getActive(): Observable<Loan[]> {
    return this.http.get<Loan[]>(this.resolveUrl('/loans/active'));
  }

  returnLoan(loanId: number): Observable<void> {
    return this.http.post<void>(this.resolveUrl(`/loans/${loanId}/return`), {});
  }

  private resolveUrl(path: string): string {
    return `${this.baseUrl.replace(/\/$/, '')}${path}`;
  }
}
