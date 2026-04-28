import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from '../api-base-url.token';
import { Book } from '../models';
import { BooksApiService } from './books-api.service';

describe('BooksApiService', () => {
  let service: BooksApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        BooksApiService,
        { provide: API_BASE_URL, useValue: '' }
      ]
    });

    service = TestBed.inject(BooksApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should request books from backend contract endpoint', () => {
    const expected: Book[] = [{ id: 1, title: 'Book', authorId: 2, availableCopies: 3 }];

    service.getBooks().subscribe((books) => {
      expect(books).toEqual(expected);
    });

    const request = httpMock.expectOne('/books');
    expect(request.request.method).toBe('GET');
    request.flush(expected);
  });
});
