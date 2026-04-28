using Libreroo.Api.Modules.Catalog.Application;
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Modules.Catalog.Api;

[ApiController]
[Route("authors")]
public sealed class AuthorsController : ControllerBase
{
    private readonly CatalogService _catalogService;

    public AuthorsController(CatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var authors = await _catalogService.GetAuthorsAsync(cancellationToken);
        return Ok(authors);
    }
}
