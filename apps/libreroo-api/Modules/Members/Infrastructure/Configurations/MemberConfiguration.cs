using Libreroo.Api.Modules.Members.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libreroo.Api.Modules.Members.Infrastructure.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");
        builder.HasKey(member => member.Id);

        builder.Property(member => member.FullName)
            .HasMaxLength(200)
            .IsRequired();
    }
}
