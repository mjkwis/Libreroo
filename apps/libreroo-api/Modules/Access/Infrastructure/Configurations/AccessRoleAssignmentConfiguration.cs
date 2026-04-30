using Libreroo.Api.Modules.Access.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libreroo.Api.Modules.Access.Infrastructure.Configurations;

public sealed class AccessRoleAssignmentConfiguration : IEntityTypeConfiguration<AccessRoleAssignment>
{
    public void Configure(EntityTypeBuilder<AccessRoleAssignment> builder)
    {
        builder.ToTable("access_role_assignments");
        builder.HasKey(roleAssignment => roleAssignment.Id);

        builder.Property(roleAssignment => roleAssignment.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(roleAssignment => roleAssignment.AssignedAtUtc)
            .IsRequired();

        builder.HasIndex(roleAssignment => new { roleAssignment.AccessUserId, roleAssignment.Role })
            .IsUnique();
    }
}
