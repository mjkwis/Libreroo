namespace Libreroo.Api.Shared.Infrastructure.Bootstrap;

public sealed class AccessBootstrapOptions
{
    public bool Enabled { get; init; }

    public string? AdminSubject { get; init; }

    public string? AdminEmail { get; init; }

    public string? AdminDisplayName { get; init; }
}
