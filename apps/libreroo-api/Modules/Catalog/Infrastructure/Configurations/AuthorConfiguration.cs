using Libreroo.Api.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libreroo.Api.Modules.Catalog.Infrastructure.Configurations;

public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("authors");
        builder.HasKey(author => author.Id);

        builder.Property(author => author.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
