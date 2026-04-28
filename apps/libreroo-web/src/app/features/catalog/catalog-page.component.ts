import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Book } from '../../core/models';
import { ApiErrorService } from '../../core/services/api-error.service';
import { BooksApiService } from '../../core/services/books-api.service';
import { LoansApiService } from '../../core/services/loans-api.service';
import { MemberContextService } from '../../core/services/member-context.service';

@Component({
  selector: 'app-catalog-page',
  imports: [CommonModule, RouterLink],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss'
})
export class CatalogPageComponent implements OnInit {
  readonly books = signal<Book[]>([]);
  readonly isLoading = signal(false);
  readonly borrowInFlightForBookId = signal<number | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  constructor(
    private readonly booksApi: BooksApiService,
    private readonly loansApi: LoansApiService,
    private readonly memberContext: MemberContextService,
    private readonly apiError: ApiErrorService
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  selectedMemberLabel(): string {
    const selectedMember = this.memberContext.selectedMember();
    return selectedMember ? `${selectedMember.fullName} (#${selectedMember.id})` : 'None selected';
  }

  canBorrow(book: Book): boolean {
    return !!this.memberContext.selectedMember() && book.availableCopies > 0;
  }

  borrow(book: Book): void {
    const selectedMember = this.memberContext.selectedMember();
    if (!selectedMember || book.availableCopies <= 0) {
      return;
    }

    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.borrowInFlightForBookId.set(book.id);

    this.loansApi
      .borrow({
        bookId: book.id,
        memberId: selectedMember.id,
        borrowDateUtc: new Date().toISOString()
      })
      .subscribe({
        next: () => {
          this.successMessage.set(`Borrowed "${book.title}".`);
          this.borrowInFlightForBookId.set(null);
          this.loadBooks();
        },
        error: (error: unknown) => {
          this.errorMessage.set(this.apiError.map(error));
          this.borrowInFlightForBookId.set(null);
        }
      });
  }

  private loadBooks(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.booksApi.getBooks().subscribe({
      next: (books) => {
        this.books.set(books);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.apiError.map(error));
        this.isLoading.set(false);
      }
    });
  }
}
