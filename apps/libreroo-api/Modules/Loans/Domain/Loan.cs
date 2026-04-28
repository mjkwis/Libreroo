namespace Libreroo.Api.Modules.Loans.Domain;

public class Loan
{
    private Loan()
    {
    }

    private Loan(int bookId, int memberId, DateTime borrowDateUtc)
    {
        BookId = bookId;
        MemberId = memberId;
        BorrowDate = borrowDateUtc;
    }

    public int Id { get; private set; }

    public int BookId { get; private set; }

    public int MemberId { get; private set; }

    public DateTime BorrowDate { get; private set; }

    public DateTime? ReturnDate { get; private set; }

    public static Loan Create(int bookId, int memberId, DateTime borrowDateUtc) =>
        new(bookId, memberId, borrowDateUtc);

    public void MarkReturned(DateTime returnDateUtc)
    {
        if (ReturnDate.HasValue)
        {
            throw new InvalidOperationException("Loan already returned.");
        }

        ReturnDate = returnDateUtc;
    }
}
