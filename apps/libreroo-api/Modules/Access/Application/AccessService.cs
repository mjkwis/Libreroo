using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Application.Errors;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Libreroo.Api.Modules.Access.Application;

public sealed class AccessService : IAccessService
{
    private readonly LibrerooDbContext _dbContext;

    public AccessService(LibrerooDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AccessUser>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.AccessUsers
            .AsNoTracking()
            .Include(accessUser => accessUser.RoleAssignments)
            .OrderBy(accessUser => accessUser.Subject)
            .ToListAsync(cancellationToken);
    }

    public async Task<AccessUser> GetCurrentUserAsync(string subject, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainRuleViolationException("Authenticated subject is required.");
        }

        var normalizedSubject = subject.Trim();

        var user = await _dbContext.AccessUsers
            .Include(accessUser => accessUser.RoleAssignments)
            .FirstOrDefaultAsync(accessUser => accessUser.Subject == normalizedSubject, cancellationToken);

        if (user is null)
        {
            throw new DomainRuleViolationException("Access user is not provisioned.");
        }

        return user;
    }

    public async Task<AccessUser> EnsureUserProfileAsync(
        string subject,
        string? email,
        string? displayName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainRuleViolationException("Authenticated subject is required.");
        }

        var normalizedSubject = subject.Trim();

        var user = await _dbContext.AccessUsers
            .Include(accessUser => accessUser.RoleAssignments)
            .FirstOrDefaultAsync(accessUser => accessUser.Subject == normalizedSubject, cancellationToken);

        if (user is null)
        {
            user = AccessUser.Create(normalizedSubject, email, displayName);
            _dbContext.AccessUsers.Add(user);
        }
        else
        {
            user.UpdateProfile(email, displayName);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<AccessUser> AssignRoleAsync(Guid userId, AccessRole role, CancellationToken cancellationToken)
    {
        var user = await _dbContext.AccessUsers
            .Include(accessUser => accessUser.RoleAssignments)
            .FirstOrDefaultAsync(accessUser => accessUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new DomainRuleViolationException("Access user not found.");
        }

        user.AssignRole(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<AccessUser> LinkMemberAsync(Guid userId, int memberId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.AccessUsers
            .Include(accessUser => accessUser.RoleAssignments)
            .FirstOrDefaultAsync(accessUser => accessUser.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new DomainRuleViolationException("Access user not found.");
        }

        var memberExists = await _dbContext.Members.AnyAsync(member => member.Id == memberId, cancellationToken);
        if (!memberExists)
        {
            throw new DomainRuleViolationException("Member not found.");
        }

        user.LinkMember(memberId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
