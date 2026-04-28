using Libreroo.Api.Modules.Catalog.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Libreroo.Api.Modules.Catalog.Application;

public sealed class CatalogService
{
    private readonly LibrerooDbContext _dbContext;

    public CatalogService(LibrerooDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Book>> GetBooksAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Books.AsNoTracking().ToListAsync(cancellationToken);

    public Task<List<Author>> GetAuthorsAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Authors.AsNoTracking().ToListAsync(cancellationToken);
}
