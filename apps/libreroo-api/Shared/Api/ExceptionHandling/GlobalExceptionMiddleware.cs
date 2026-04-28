using System.Net;
using Libreroo.Api.Shared.Application.Errors;

namespace Libreroo.Api.Shared.Api.ExceptionHandling;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainRuleViolationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    }
}
