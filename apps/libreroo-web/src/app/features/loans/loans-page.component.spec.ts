import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/services/api-error.service';
import { LoansApiService } from '../../core/services/loans-api.service';
import { LoansPageComponent } from './loans-page.component';

describe('LoansPageComponent', () => {
  let fixture: ComponentFixture<LoansPageComponent>;
  let loansApi: jasmine.SpyObj<LoansApiService>;

  beforeEach(async () => {
    loansApi = jasmine.createSpyObj<LoansApiService>('LoansApiService', ['getActive', 'returnLoan']);
    loansApi.getActive.and.returnValues(
      of([{ id: 11, bookId: 7, memberId: 3, borrowDate: '2026-04-28T10:00:00Z', returnDate: null }]),
      of([])
    );
    loansApi.returnLoan.and.returnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [LoansPageComponent],
      providers: [{ provide: LoansApiService, useValue: loansApi }, ApiErrorService]
    }).compileComponents();

    fixture = TestBed.createComponent(LoansPageComponent);
    fixture.detectChanges();
  });

  it('should return active loan and refresh list', () => {
    const button = fixture.nativeElement.querySelector('tbody button') as HTMLButtonElement;
    button.click();
    fixture.detectChanges();

    expect(loansApi.returnLoan).toHaveBeenCalledWith(11);
    expect(loansApi.getActive).toHaveBeenCalledTimes(2);
  });
});
