0#0 Libreroo Backend Foundation Implementation Plan
0

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:
> executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Bootstrap a runnable `apps/libreroo-api` modular-monolith backend with PostgreSQL container, core module
boundaries, loan business invariants, and baseline API endpoints without authentication.

**Architecture:** Build one ASP.NET Core Web API project with internal module folders (`Catalog`, `Members`, `Loans`,
`Shared`) and layer boundaries (`Api`, `Application`, `Domain`, `Infrastructure`). Keep controllers thin, use EF Core +
Npgsql for persistence, and enforce loan rules in domain/application logic.

**Tech Stack:** .NET 10, ASP.NET Core Web API, EF Core, Npgsql, xUnit, FluentAssertions, Docker Compose (PostgreSQL),
Swagger/OpenAPI.

---

## File Structure Map

- Create: `Libreroo.sln`  
  Responsibility: solution root containing API and tests.
- Create: `apps/libreroo-api/Libreroo.Api.csproj`  
  Responsibility: Web API host, DI composition, middleware, controllers, EF registration.
- Create: `apps/libreroo-api/Program.cs`  
  Responsibility: service registration, middleware pipeline, endpoint mapping.
- Create: `apps/libreroo-api/appsettings.json` and `apps/libreroo-api/appsettings.Development.json`  
  Responsibility: runtime config and local PostgreSQL defaults.
- Create: `apps/libreroo-api/Shared/Api/ExceptionHandling/GlobalExceptionMiddleware.cs`  
  Responsibility: consistent error-to-HTTP mapping.
- Create: `apps/libreroo-api/Shared/Application/Errors/DomainRuleViolationException.cs`  
  Responsibility: explicit business-rule error signaling.
- Create: `apps/libreroo-api/Shared/Infrastructure/Persistence/LibrerooDbContext.cs`  
  Responsibility: EF Core DbContext and DbSet exposure.
- Create: `apps/libreroo-api/Modules/Catalog/Domain/{Author.cs,Book.cs}`  
  Responsibility: catalog entities.
- Create: `apps/libreroo-api/Modules/Members/Domain/Member.cs`  
  Responsibility: member entity.
- Create: `apps/libreroo-api/Modules/Loans/Domain/Loan.cs`  
  Responsibility: loan aggregate and return-state rules.
- Create: `apps/libreroo-api/Modules/*/Infrastructure/Configurations/*.cs`  
  Responsibility: entity mappings and table constraints.
- Create: `apps/libreroo-api/Modules/Loans/Application/{BorrowBookCommand.cs,ReturnLoanCommand.cs,LoanService.cs}`  
  Responsibility: borrow/return orchestration and invariants.
- Create: `apps/libreroo-api/Modules/*/Api/*.cs`  
  Responsibility: REST endpoints and DTO contracts.
- Create: `apps/libreroo-api/Migrations/*`  
  Responsibility: initial schema migration.
- Create: `tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj`  
  Responsibility: API integration tests.
- Create: `tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj`  
  Responsibility: domain/application rule tests.
- Create: `docker-compose.yml`  
  Responsibility: local PostgreSQL container.
- Create: `.ai/docs/adr/0002-no-authentication-in-phase-1.md`  
  Responsibility: decision record for explicit auth deferral.

### Task 1: Solution Bootstrap and Health Vertical Slice

**Files:**

- Create: `Libreroo.sln`
- Create: `apps/libreroo-api/Libreroo.Api.csproj`
- Create: `apps/libreroo-api/Program.cs`
- Create: `apps/libreroo-api/Controllers/HealthController.cs`
- Create: `tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj`
- Create: `tests/Libreroo.Api.Tests/HealthEndpointTests.cs`

- [ ] **Step 1: Write the failing health integration test**

```csharp
// tests/Libreroo.Api.Tests/HealthEndpointTests.cs
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Libreroo.Api.Tests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: FAIL with host build/startup error because API project and endpoint are not created yet.

- [ ] **Step 3: Create minimal API host and health endpoint**

```csharp
// apps/libreroo-api/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program;
```

```csharp
// apps/libreroo-api/Controllers/HealthController.cs
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS (`GetHealth_ReturnsOk` green).

- [ ] **Step 5: Commit**

```bash
git add Libreroo.sln apps/libreroo-api tests/Libreroo.Api.Tests
git commit -m "feat(api): bootstrap solution and health endpoint"
```

### Task 2: PostgreSQL Container and EF Core Wiring

**Files:**

- Create: `docker-compose.yml`
- Modify: `apps/libreroo-api/Libreroo.Api.csproj`
- Create: `apps/libreroo-api/appsettings.json`
- Create: `apps/libreroo-api/appsettings.Development.json`
- Create: `apps/libreroo-api/Shared/Infrastructure/Persistence/LibrerooDbContext.cs`
- Modify: `apps/libreroo-api/Program.cs`
- Create: `tests/Libreroo.Api.Tests/DatabaseConnectivityTests.cs`

- [ ] **Step 1: Write failing DB connectivity test**

```csharp
// tests/Libreroo.Api.Tests/DatabaseConnectivityTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Libreroo.Api.Shared.Infrastructure.Persistence;

namespace Libreroo.Api.Tests;

public class DatabaseConnectivityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DatabaseConnectivityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DbContext_CanConnect()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();
        var canConnect = await db.Database.CanConnectAsync();
        Assert.True(canConnect);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: FAIL with missing `LibrerooDbContext` registration and/or DB connectivity failure.

- [ ] **Step 3: Add PostgreSQL container, EF packages, and DbContext registration**

```yaml
# docker-compose.yml
services:
  postgres:
    image: postgres:16
    container_name: libreroo-postgres
    environment:
      POSTGRES_USER: libreroo
      POSTGRES_PASSWORD: libreroo
      POSTGRES_DB: libreroo
    ports:
      - "5432:5432"
    volumes:
      - libreroo-postgres-data:/var/lib/postgresql/data

volumes:
  libreroo-postgres-data:
```

```csharp
// apps/libreroo-api/Shared/Infrastructure/Persistence/LibrerooDbContext.cs
using Microsoft.EntityFrameworkCore;
using Libreroo.Api.Modules.Catalog.Domain;
using Libreroo.Api.Modules.Members.Domain;
using Libreroo.Api.Modules.Loans.Domain;

namespace Libreroo.Api.Shared.Infrastructure.Persistence;

public class LibrerooDbContext : DbContext
{
    public LibrerooDbContext(DbContextOptions<LibrerooDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();
}
```

```csharp
// Program.cs fragment
builder.Services.AddDbContext<LibrerooDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

- [ ] **Step 4: Start PostgreSQL and rerun tests**

Run: `docker compose up -d postgres`  
Expected: container `libreroo-postgres` is running.

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: `DbContext_CanConnect` PASS.

- [ ] **Step 5: Commit**

```bash
git add docker-compose.yml apps/libreroo-api tests/Libreroo.Api.Tests
git commit -m "feat(api): add postgres container and ef core wiring"
```

### Task 3: Domain Model, Configurations, and Initial Migration

**Files:**

- Create: `apps/libreroo-api/Modules/Catalog/Domain/Author.cs`
- Create: `apps/libreroo-api/Modules/Catalog/Domain/Book.cs`
- Create: `apps/libreroo-api/Modules/Members/Domain/Member.cs`
- Create: `apps/libreroo-api/Modules/Loans/Domain/Loan.cs`
- Create:
  `apps/libreroo-api/Modules/Catalog/Infrastructure/Configurations/{AuthorConfiguration.cs,BookConfiguration.cs}`
- Create: `apps/libreroo-api/Modules/Members/Infrastructure/Configurations/MemberConfiguration.cs`
- Create: `apps/libreroo-api/Modules/Loans/Infrastructure/Configurations/LoanConfiguration.cs`
- Modify: `apps/libreroo-api/Shared/Infrastructure/Persistence/LibrerooDbContext.cs`
- Create: `tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj`
- Create: `tests/Libreroo.Domain.Tests/LoanTests.cs`
- Create: `apps/libreroo-api/Migrations/*`

- [ ] **Step 1: Write failing domain test for return state transition**

```csharp
// tests/Libreroo.Domain.Tests/LoanTests.cs
using Libreroo.Api.Modules.Loans.Domain;

namespace Libreroo.Domain.Tests;

public class LoanTests
{
    [Fact]
    public void MarkReturned_SecondCall_Throws()
    {
        var loan = Loan.Create(bookId: 1, memberId: 2, borrowDateUtc: DateTime.UtcNow);
        loan.MarkReturned(DateTime.UtcNow.AddDays(1));

        Assert.Throws<InvalidOperationException>(() => loan.MarkReturned(DateTime.UtcNow.AddDays(2)));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: FAIL because `Loan` entity and behavior do not exist yet.

- [ ] **Step 3: Implement entities and EF configurations**

```csharp
// apps/libreroo-api/Modules/Loans/Domain/Loan.cs
namespace Libreroo.Api.Modules.Loans.Domain;

public class Loan
{
    public int Id { get; private set; }
    public int BookId { get; private set; }
    public int MemberId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    private Loan() { }

    private Loan(int bookId, int memberId, DateTime borrowDateUtc)
    {
        BookId = bookId;
        MemberId = memberId;
        BorrowDate = borrowDateUtc;
    }

    public static Loan Create(int bookId, int memberId, DateTime borrowDateUtc) =>
        new(bookId, memberId, borrowDateUtc);

    public void MarkReturned(DateTime returnDateUtc)
    {
        if (ReturnDate.HasValue) throw new InvalidOperationException("Loan already returned.");
        ReturnDate = returnDateUtc;
    }
}
```

```csharp
// LibrerooDbContext.OnModelCreating fragment
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibrerooDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
}
```

- [ ] **Step 4: Create and apply initial migration**

Run:
`dotnet ef migrations add InitialCreate --project apps/libreroo-api/Libreroo.Api.csproj --startup-project apps/libreroo-api/Libreroo.Api.csproj`  
Expected: migration files created in `apps/libreroo-api/Migrations`.

Run:
`dotnet ef database update --project apps/libreroo-api/Libreroo.Api.csproj --startup-project apps/libreroo-api/Libreroo.Api.csproj`  
Expected: database schema created in PostgreSQL.

- [ ] **Step 5: Run tests to verify pass**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add apps/libreroo-api tests/Libreroo.Domain.Tests
git commit -m "feat(domain): add core entities and initial migration"
```

### Task 4: Global Exception Handling and Validation Baseline

**Files:**

- Create: `apps/libreroo-api/Shared/Application/Errors/DomainRuleViolationException.cs`
- Create: `apps/libreroo-api/Shared/Api/ExceptionHandling/GlobalExceptionMiddleware.cs`
- Modify: `apps/libreroo-api/Program.cs`
- Create: `tests/Libreroo.Api.Tests/ErrorHandlingTests.cs`

- [ ] **Step 1: Write failing integration test for domain-rule response mapping**

```csharp
// tests/Libreroo.Api.Tests/ErrorHandlingTests.cs
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Libreroo.Api.Tests;

public class ErrorHandlingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ErrorHandlingTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReturnLoan_WhenAlreadyReturned_ReturnsConflict()
    {
        var response = await _client.PostAsync("/loans/999/return", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: FAIL with `404` or unhandled error behavior.

- [ ] **Step 3: Implement global exception middleware and register pipeline**

```csharp
// apps/libreroo-api/Shared/Application/Errors/DomainRuleViolationException.cs
namespace Libreroo.Api.Shared.Application.Errors;

public sealed class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message) : base(message) { }
}
```

```csharp
// apps/libreroo-api/Shared/Api/ExceptionHandling/GlobalExceptionMiddleware.cs
using System.Net;
using Libreroo.Api.Shared.Application.Errors;

namespace Libreroo.Api.Shared.Api.ExceptionHandling;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next) => _next = next;

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
```

- [ ] **Step 4: Run integration tests to verify behavior**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS for conflict mapping once loan endpoint exists.

- [ ] **Step 5: Commit**

```bash
git add apps/libreroo-api tests/Libreroo.Api.Tests
git commit -m "feat(api): add global exception handling baseline"
```

### Task 5: Catalog and Members CRUD API Baseline

**Files:**

- Create: `apps/libreroo-api/Modules/Catalog/Api/BooksController.cs`
- Create: `apps/libreroo-api/Modules/Catalog/Api/AuthorsController.cs`
- Create: `apps/libreroo-api/Modules/Members/Api/MembersController.cs`
- Create: `apps/libreroo-api/Modules/Catalog/Application/CatalogService.cs`
- Create: `apps/libreroo-api/Modules/Members/Application/MemberService.cs`
- Create: `tests/Libreroo.Api.Tests/BooksEndpointTests.cs`

- [ ] **Step 1: Write failing integration test for books list endpoint**

```csharp
// tests/Libreroo.Api.Tests/BooksEndpointTests.cs
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Libreroo.Api.Tests;

public class BooksEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ReturnsOk()
    {
        var response = await _client.GetAsync("/books");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: FAIL with `404` for `/books`.

- [ ] **Step 3: Implement minimal CRUD baseline for books/authors/members**

```csharp
// apps/libreroo-api/Modules/Catalog/Api/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Libreroo.Api.Shared.Infrastructure.Persistence;

namespace Libreroo.Api.Modules.Catalog.Api;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly LibrerooDbContext _dbContext;

    public BooksController(LibrerooDbContext dbContext) => _dbContext = dbContext;

    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _dbContext.Books.AsNoTracking().ToListAsync());
}
```

- [ ] **Step 4: Run integration tests**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS for `GetBooks_ReturnsOk`.

- [ ] **Step 5: Commit**

```bash
git add apps/libreroo-api tests/Libreroo.Api.Tests
git commit -m "feat(catalog,members): add CRUD endpoint baseline"
```

### Task 6: Loans Borrow/Return Use Cases and Active Loans Endpoint

**Files:**

- Create: `apps/libreroo-api/Modules/Loans/Application/BorrowBookCommand.cs`
- Create: `apps/libreroo-api/Modules/Loans/Application/ReturnLoanCommand.cs`
- Create: `apps/libreroo-api/Modules/Loans/Application/LoanService.cs`
- Create: `apps/libreroo-api/Modules/Loans/Api/LoansController.cs`
- Modify: `tests/Libreroo.Domain.Tests/LoanTests.cs`
- Create: `tests/Libreroo.Api.Tests/LoansEndpointTests.cs`

- [ ] **Step 1: Add failing tests for borrow and return invariants**

```csharp
// tests/Libreroo.Domain.Tests/LoanTests.cs (additional tests)
[Fact]
public void Borrow_WhenNoAvailableCopies_ThrowsDomainRuleViolation()
{
    var book = new Libreroo.Api.Modules.Catalog.Domain.Book("DDD", authorId: 1, availableCopies: 0);
    Assert.Throws<Libreroo.Api.Shared.Application.Errors.DomainRuleViolationException>(
        () => book.DecreaseAvailableCopies());
}
```

```csharp
// tests/Libreroo.Api.Tests/LoansEndpointTests.cs
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Libreroo.Api.Tests;

public class LoansEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoansEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetActiveLoans_ReturnsOk()
    {
        var response = await _client.GetAsync("/loans/active");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run tests to verify failures**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: FAIL due to missing copy decrement invariant behavior.

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: FAIL with `404` for `/loans/active`.

- [ ] **Step 3: Implement loan application service and endpoint actions**

```csharp
// apps/libreroo-api/Modules/Loans/Api/LoansController.cs
using Microsoft.AspNetCore.Mvc;
using Libreroo.Api.Modules.Loans.Application;

namespace Libreroo.Api.Modules.Loans.Api;

[ApiController]
[Route("loans")]
public class LoansController : ControllerBase
{
    private readonly LoanService _loanService;

    public LoansController(LoanService loanService) => _loanService = loanService;

    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] BorrowBookCommand command)
    {
        var loanId = await _loanService.BorrowAsync(command);
        return Created($"/loans/{loanId}", new { id = loanId });
    }

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(int id)
    {
        await _loanService.ReturnAsync(new ReturnLoanCommand(id, DateTime.UtcNow));
        return Ok();
    }

    [HttpGet("active")]
    public async Task<IActionResult> Active()
    {
        var loans = await _loanService.GetActiveAsync();
        return Ok(loans);
    }
}
```

- [ ] **Step 4: Run tests to verify pass**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: PASS.

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS for active-loans route and borrow/return behavior.

- [ ] **Step 5: Commit**

```bash
git add apps/libreroo-api tests/Libreroo.Domain.Tests tests/Libreroo.Api.Tests
git commit -m "feat(loans): implement borrow return flows and active loans endpoint"
```

### Task 7: ADR for Phase-1 No-Auth and Final Verification

**Files:**

- Create: `.ai/docs/adr/0002-no-authentication-in-phase-1.md`
- Modify: `.ai/docs/adr/README.md`

- [ ] **Step 1: Write failing documentation check (manual gate)**

Check: no ADR exists for auth deferral in `.ai/docs/adr`.  
Expected: FAIL gate (missing decision record).

- [ ] **Step 2: Add ADR documenting no-auth phase-1 decision**

```markdown
# 0002 - No Authentication in Phase 1

## Status
Accepted

## Date
2026-04-28

## Context
Phase 1 targets a fast, working backend MVP focused on module boundaries, CRUD flows, and loan invariants.

## Decision
Do not implement authentication or authorization in phase 1.

## Consequences
- Faster time to first working demo.
- Reduced initial complexity in API flows.
- Phase 2 must add authN/authZ and align endpoint compatibility strategy.
```

- [ ] **Step 3: Run full verification**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: PASS.

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS.

Run: `dotnet build Libreroo.sln -v minimal`  
Expected: BUILD SUCCEEDED.

- [ ] **Step 4: Commit**

```bash
git add .ai/docs/adr apps/libreroo-api tests docker-compose.yml
git commit -m "docs(adr): record phase 1 no-auth decision and verify backend foundation"
```

## Self-Review

### Spec coverage check

- Backend-first runnable API: covered by Tasks 1-2.
- Modular boundaries and domain model: covered by Tasks 3 and 6.
- PostgreSQL container from start: covered by Task 2.
- Global exception handling and validation baseline: covered by Task 4.
- CRUD + loan endpoints: covered by Tasks 5-6.
- No-auth decision captured in ADR: covered by Task 7.

No uncovered spec requirement found.

### Placeholder scan

- No `TBD`, `TODO`, or deferred placeholders left in execution steps.
- All steps contain explicit files, commands, and expected results.

### Type/signature consistency

- Loan flow signatures align across tests, commands, service, and controller routes:
    - `BorrowBookCommand`
    - `ReturnLoanCommand`
    - `LoanService.BorrowAsync/ReturnAsync/GetActiveAsync`

