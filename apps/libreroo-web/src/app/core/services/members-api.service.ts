import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url.token';
import { CreateMemberRequest, Member } from '../models';

@Injectable({ providedIn: 'root' })
export class MembersApiService {
  constructor(
    private readonly http: HttpClient,
    @Inject(API_BASE_URL) private readonly baseUrl: string
  ) {}

  getMembers(): Observable<Member[]> {
    return this.http.get<Member[]>(this.resolveUrl('/members'));
  }

  createMember(request: CreateMemberRequest): Observable<Member> {
    return this.http.post<Member>(this.resolveUrl('/members'), request);
  }

  private resolveUrl(path: string): string {
    return `${this.baseUrl.replace(/\/$/, '')}${path}`;
  }
}
