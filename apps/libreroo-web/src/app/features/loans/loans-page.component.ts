import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Loan } from '../../core/models';
import { ApiErrorService } from '../../core/services/api-error.service';
import { LoansApiService } from '../../core/services/loans-api.service';

@Component({
  selector: 'app-loans-page',
  imports: [CommonModule, DatePipe],
  templateUrl: './loans-page.component.html',
  styleUrl: './loans-page.component.scss'
})
export class LoansPageComponent implements OnInit {
  readonly loans = signal<Loan[]>([]);
  readonly isLoading = signal(false);
  readonly returningLoanId = signal<number | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  constructor(
    private readonly loansApi: LoansApiService,
    private readonly apiError: ApiErrorService
  ) {}

  ngOnInit(): void {
    this.loadActiveLoans();
  }

  returnLoan(loanId: number): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.returningLoanId.set(loanId);

    this.loansApi.returnLoan(loanId).subscribe({
      next: () => {
        this.successMessage.set(`Returned loan #${loanId}.`);
        this.returningLoanId.set(null);
        this.loadActiveLoans();
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.apiError.map(error));
        this.returningLoanId.set(null);
      }
    });
  }

  private loadActiveLoans(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.loansApi.getActive().subscribe({
      next: (loans) => {
        this.loans.set(loans);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.apiError.map(error));
        this.isLoading.set(false);
      }
    });
  }
}
