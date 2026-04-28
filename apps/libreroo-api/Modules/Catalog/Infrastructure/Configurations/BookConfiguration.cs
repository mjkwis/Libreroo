using Libreroo.Api.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libreroo.Api.Modules.Catalog.Infrastructure.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");
        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(book => book.AvailableCopies)
            .IsRequired();

        builder.HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
            table.HasCheckConstraint("CK_books_available_copies_non_negative", "\"AvailableCopies\" >= 0"));
    }
}
