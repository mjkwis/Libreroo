using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Modules.Catalog.Domain;
using Libreroo.Api.Modules.Loans.Domain;
using Libreroo.Api.Modules.Members.Domain;
using Microsoft.EntityFrameworkCore;

namespace Libreroo.Api.Shared.Infrastructure.Persistence;

public class LibrerooDbContext : DbContext
{
    public LibrerooDbContext(DbContextOptions<LibrerooDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Loan> Loans => Set<Loan>();

    public DbSet<AccessUser> AccessUsers => Set<AccessUser>();

    public DbSet<AccessRoleAssignment> AccessRoleAssignments => Set<AccessRoleAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibrerooDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
