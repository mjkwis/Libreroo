import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url.token';
import { Book } from '../models';

@Injectable({ providedIn: 'root' })
export class BooksApiService {
  constructor(
    private readonly http: HttpClient,
    @Inject(API_BASE_URL) private readonly baseUrl: string
  ) {}

  getBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(this.resolveUrl('/books'));
  }

  private resolveUrl(path: string): string {
    return `${this.baseUrl.replace(/\/$/, '')}${path}`;
  }
}
