using Libreroo.Api.Modules.Access.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libreroo.Api.Modules.Access.Infrastructure.Configurations;

public sealed class AccessUserConfiguration : IEntityTypeConfiguration<AccessUser>
{
    public void Configure(EntityTypeBuilder<AccessUser> builder)
    {
        builder.ToTable("access_users");
        builder.HasKey(accessUser => accessUser.Id);

        builder.Property(accessUser => accessUser.Subject)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(accessUser => accessUser.Email)
            .HasMaxLength(320)
            .IsRequired(false);

        builder.Property(accessUser => accessUser.DisplayName)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(accessUser => accessUser.MemberId)
            .IsRequired(false);

        builder.HasIndex(accessUser => accessUser.Subject)
            .IsUnique();

        builder.HasIndex(accessUser => accessUser.MemberId)
            .IsUnique()
            .HasFilter("\"MemberId\" IS NOT NULL");

        builder.HasMany(accessUser => accessUser.RoleAssignments)
            .WithOne(roleAssignment => roleAssignment.AccessUser)
            .HasForeignKey(roleAssignment => roleAssignment.AccessUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
