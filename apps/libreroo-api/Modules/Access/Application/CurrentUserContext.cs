using Libreroo.Api.Modules.Access.Domain;

namespace Libreroo.Api.Modules.Access.Application;

public sealed class CurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessService _accessService;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor, IAccessService accessService)
    {
        _httpContextAccessor = httpContextAccessor;
        _accessService = accessService;
    }

    public string? Subject => _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

    public Task<AccessUser> GetRequiredCurrentUserAsync(CancellationToken cancellationToken)
    {
        return _accessService.GetCurrentUserAsync(Subject ?? string.Empty, cancellationToken);
    }

    public Task<AccessUser> EnsureCurrentUserProfileAsync(CancellationToken cancellationToken)
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        var subject = principal?.FindFirst("sub")?.Value ?? string.Empty;
        var email = principal?.FindFirst("email")?.Value;
        var displayName = principal?.FindFirst("name")?.Value;
        return _accessService.EnsureUserProfileAsync(subject, email, displayName, cancellationToken);
    }
}
