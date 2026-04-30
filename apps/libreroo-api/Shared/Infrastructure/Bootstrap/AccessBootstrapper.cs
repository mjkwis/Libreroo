using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Libreroo.Api.Shared.Infrastructure.Bootstrap;

public sealed class AccessBootstrapper
{
    private readonly LibrerooDbContext _dbContext;
    private readonly AccessBootstrapOptions _options;
    private readonly ILogger<AccessBootstrapper> _logger;

    public AccessBootstrapper(
        LibrerooDbContext dbContext,
        IOptions<AccessBootstrapOptions> options,
        ILogger<AccessBootstrapper> logger)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _logger = logger;
    }

    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.AdminSubject))
        {
            _logger.LogWarning("Access bootstrap is enabled but AccessBootstrap:AdminSubject is missing.");
            return;
        }

        var subject = _options.AdminSubject.Trim();

        var admin = await _dbContext.Set<AccessUser>()
            .Include(user => user.RoleAssignments)
            .FirstOrDefaultAsync(user => user.Subject == subject, cancellationToken);

        if (admin is null)
        {
            admin = AccessUser.Create(subject, _options.AdminEmail, _options.AdminDisplayName ?? "Libreroo Admin");
            _dbContext.Set<AccessUser>().Add(admin);
        }
        else
        {
            admin.UpdateProfile(_options.AdminEmail, _options.AdminDisplayName);
        }

        admin.AssignRole(AccessRole.Admin);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
