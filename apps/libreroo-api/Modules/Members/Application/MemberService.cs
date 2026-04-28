using Libreroo.Api.Modules.Members.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Libreroo.Api.Modules.Members.Application;

public sealed class MemberService
{
    private readonly LibrerooDbContext _dbContext;

    public MemberService(LibrerooDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Member>> GetMembersAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Members.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Member> CreateMemberAsync(string fullName, CancellationToken cancellationToken = default)
    {
        var member = new Member(fullName);
        _dbContext.Members.Add(member);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return member;
    }
}
