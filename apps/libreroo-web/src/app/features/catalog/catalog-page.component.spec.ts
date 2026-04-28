import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/services/api-error.service';
import { BooksApiService } from '../../core/services/books-api.service';
import { LoansApiService } from '../../core/services/loans-api.service';
import { MemberContextService } from '../../core/services/member-context.service';
import { CatalogPageComponent } from './catalog-page.component';

describe('CatalogPageComponent', () => {
  let fixture: ComponentFixture<CatalogPageComponent>;
  let booksApi: jasmine.SpyObj<BooksApiService>;
  let loansApi: jasmine.SpyObj<LoansApiService>;
  let memberContext: { selectedMember: jasmine.Spy };

  beforeEach(async () => {
    booksApi = jasmine.createSpyObj<BooksApiService>('BooksApiService', ['getBooks']);
    loansApi = jasmine.createSpyObj<LoansApiService>('LoansApiService', ['borrow']);
    memberContext = {
      selectedMember: jasmine.createSpy('selectedMember')
    };

    booksApi.getBooks.and.returnValue(of([{ id: 1, title: 'Book A', authorId: 8, availableCopies: 2 }]));
    loansApi.borrow.and.returnValue(of({ id: 99 }));
    memberContext.selectedMember.and.returnValue(null);

    await TestBed.configureTestingModule({
      imports: [CatalogPageComponent],
      providers: [
        { provide: BooksApiService, useValue: booksApi },
        { provide: LoansApiService, useValue: loansApi },
        { provide: MemberContextService, useValue: memberContext },
        ApiErrorService,
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CatalogPageComponent);
    fixture.detectChanges();
  });

  it('should disable borrow action when no active member is selected', () => {
    const borrowButton = fixture.nativeElement.querySelector('tbody button') as HTMLButtonElement;
    expect(borrowButton.disabled).toBeTrue();
  });

  it('should send borrow payload with selected member id and book id', () => {
    memberContext.selectedMember.and.returnValue({ id: 7, fullName: 'Active Member' });
    fixture = TestBed.createComponent(CatalogPageComponent);
    fixture.detectChanges();

    const borrowButton = fixture.nativeElement.querySelector('tbody button') as HTMLButtonElement;
    borrowButton.click();

    expect(loansApi.borrow).toHaveBeenCalledWith(
      jasmine.objectContaining({
        bookId: 1,
        memberId: 7,
        borrowDateUtc: jasmine.any(String)
      })
    );
  });
});
