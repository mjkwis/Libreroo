import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from '../api-base-url.token';
import { Loan } from '../models';
import { LoansApiService } from './loans-api.service';

describe('LoansApiService', () => {
  let service: LoansApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        LoansApiService,
        { provide: API_BASE_URL, useValue: '' }
      ]
    });

    service = TestBed.inject(LoansApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should post borrow payload with backend contract fields', () => {
    service.borrow({ bookId: 5, memberId: 7, borrowDateUtc: '2026-04-28T10:30:00.000Z' }).subscribe((result) => {
      expect(result).toEqual({ id: 33 });
    });

    const request = httpMock.expectOne('/loans');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      bookId: 5,
      memberId: 7,
      borrowDateUtc: '2026-04-28T10:30:00.000Z'
    });
    request.flush({ id: 33 });
  });

  it('should request active loans and post return action', () => {
    const expectedLoans: Loan[] = [
      { id: 3, bookId: 2, memberId: 7, borrowDate: '2026-04-28T10:00:00Z', returnDate: null }
    ];

    service.getActive().subscribe((loans) => {
      expect(loans).toEqual(expectedLoans);
    });

    const activeRequest = httpMock.expectOne('/loans/active');
    expect(activeRequest.request.method).toBe('GET');
    activeRequest.flush(expectedLoans);

    service.returnLoan(3).subscribe((result) => {
      expect(result).toBeNull();
    });

    const returnRequest = httpMock.expectOne('/loans/3/return');
    expect(returnRequest.request.method).toBe('POST');
    returnRequest.flush(null);
  });
});
