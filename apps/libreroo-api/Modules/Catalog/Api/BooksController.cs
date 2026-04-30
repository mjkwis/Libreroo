using Libreroo.Api.Modules.Catalog.Application;
using Libreroo.Api.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Modules.Catalog.Api;

[ApiController]
[Route("books")]
[Authorize(Policy = AccessPolicies.MemberOrAbove)]
public sealed class BooksController : ControllerBase
{
    private readonly CatalogService _catalogService;

    public BooksController(CatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var books = await _catalogService.GetBooksAsync(cancellationToken);
        return Ok(books);
    }
}
