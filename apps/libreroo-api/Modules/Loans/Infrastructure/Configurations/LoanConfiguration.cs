using Libreroo.Api.Modules.Loans.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libreroo.Api.Modules.Loans.Infrastructure.Configurations;

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("loans");
        builder.HasKey(loan => loan.Id);

        builder.Property(loan => loan.BorrowDate)
            .IsRequired();

        builder.Property(loan => loan.ReturnDate)
            .IsRequired(false);

        builder.HasIndex(loan => new { loan.BookId, loan.ReturnDate });
        builder.HasIndex(loan => new { loan.MemberId, loan.ReturnDate });
    }
}
