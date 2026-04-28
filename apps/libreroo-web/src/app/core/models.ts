export interface Book {
  id: number;
  title: string;
  authorId: number;
  availableCopies: number;
}

export interface Member {
  id: number;
  fullName: string;
}

export interface Loan {
  id: number;
  bookId: number;
  memberId: number;
  borrowDate: string;
  returnDate: string | null;
}

export interface BorrowBookRequest {
  bookId: number;
  memberId: number;
  borrowDateUtc: string;
}

export interface CreateMemberRequest {
  fullName: string;
}

export interface SelectedMember {
  id: number;
  fullName: string;
}
