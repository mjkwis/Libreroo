namespace Libreroo.Api.Modules.Loans.Application;

public sealed record BorrowBookCommand(int BookId, int MemberId, DateTime BorrowDateUtc);
