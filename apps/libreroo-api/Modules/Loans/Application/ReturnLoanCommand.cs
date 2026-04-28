namespace Libreroo.Api.Modules.Loans.Application;

public sealed record ReturnLoanCommand(int LoanId, DateTime ReturnDateUtc);
