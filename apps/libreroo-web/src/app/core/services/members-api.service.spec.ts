import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from '../api-base-url.token';
import { Member } from '../models';
import { MembersApiService } from './members-api.service';

describe('MembersApiService', () => {
  let service: MembersApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        MembersApiService,
        { provide: API_BASE_URL, useValue: '' }
      ]
    });

    service = TestBed.inject(MembersApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should request members from backend contract endpoint', () => {
    const expected: Member[] = [{ id: 9, fullName: 'Ada Lovelace' }];

    service.getMembers().subscribe((members) => {
      expect(members).toEqual(expected);
    });

    const request = httpMock.expectOne('/members');
    expect(request.request.method).toBe('GET');
    request.flush(expected);
  });

  it('should post member creation payload', () => {
    const expected: Member = { id: 10, fullName: 'Alan Turing' };

    service.createMember({ fullName: 'Alan Turing' }).subscribe((member) => {
      expect(member).toEqual(expected);
    });

    const request = httpMock.expectOne('/members');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ fullName: 'Alan Turing' });
    request.flush(expected);
  });
});
