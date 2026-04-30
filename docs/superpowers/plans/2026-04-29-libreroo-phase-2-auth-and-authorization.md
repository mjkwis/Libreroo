# Libreroo Phase 2 Auth and Authorization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement Phase 2 security with Keycloak authentication and PostgreSQL-authoritative authorization for `member`, `librarian`, and `admin`.

**Architecture:** Keycloak is identity proof and token issuer only. Libreroo `Access` module is the sole source of authorization truth (roles, member linkage, permissions). API validates JWTs, resolves local user/access records by `sub`, and enforces policies through DB-backed handlers.

**Tech Stack:** .NET 10, ASP.NET Core JWT Bearer + policy handlers, EF Core + Npgsql, Angular 20, OIDC Code + PKCE, Docker Compose, xUnit.

---

## File Structure Map

- Modify: `docker-compose.yml`  
  Responsibility: add local Keycloak runtime.
- Create: `docs/keycloak/realm-export/libreroo-realm.json`  
  Responsibility: local realm/client/roles bootstrap.
- Modify: `apps/libreroo-api/appsettings.json`
- Modify: `apps/libreroo-api/appsettings.Development.json`  
  Responsibility: auth options and bootstrap config (no hardcoded secrets).
- Modify: `apps/libreroo-api/Program.cs`  
  Responsibility: authentication, authorization, middleware, policy registration.
- Create: `apps/libreroo-api/Modules/Access/Domain/{AccessRole.cs,AccessUser.cs,AccessRoleAssignment.cs}`
- Create: `apps/libreroo-api/Modules/Access/Infrastructure/Configurations/{AccessUserConfiguration.cs,AccessRoleAssignmentConfiguration.cs}`
- Create: `apps/libreroo-api/Modules/Access/Application/{IAccessService.cs,AccessService.cs,CurrentUserContext.cs}`
- Create: `apps/libreroo-api/Modules/Access/Api/{AccessUsersController.cs,AccessDtos.cs}`
- Create: `apps/libreroo-api/Shared/Security/{AccessPolicies.cs,AccessPolicyRequirements.cs,AccessPolicyHandlers.cs}`
- Create: `apps/libreroo-api/Shared/Infrastructure/Bootstrap/AccessBootstrapOptions.cs`
- Create: `apps/libreroo-api/Shared/Infrastructure/Bootstrap/AccessBootstrapper.cs`
- Modify: `apps/libreroo-api/Shared/Infrastructure/Persistence/LibrerooDbContext.cs`
- Modify: `apps/libreroo-api/Modules/Catalog/Api/*.cs`
- Modify: `apps/libreroo-api/Modules/Members/Api/*.cs`
- Modify: `apps/libreroo-api/Modules/Loans/{Api,Application}/*.cs`
- Modify: `tests/Libreroo.Api.Tests/CustomWebApplicationFactory.cs`
- Create: `tests/Libreroo.Api.Tests/Auth/{AuthPolicyTests.cs,LoanAuthorizationTests.cs,AccessAdminEndpointsTests.cs}`
- Create: `tests/Libreroo.Api.Tests/Auth/KeycloakJwtValidationTests.cs`
- Create: `tests/Libreroo.Domain.Tests/Access/AccessServiceTests.cs`
- Create: `apps/libreroo-web/src/app/core/auth/{auth.service.ts,auth.guard.ts,role.guard.ts,auth.interceptor.ts,models.ts}`
- Modify: `apps/libreroo-web/src/app/{app.config.ts,app.routes.ts,app.html}`
- Modify: `apps/libreroo-web/src/app/core/services/{loans-api.service.ts,member-context.service.ts}`
- Modify: `apps/libreroo-web/src/app/features/{catalog,member,loans}/*`
- Create: `apps/libreroo-web/src/app/features/access/access-page.component.{ts,html,scss,spec.ts}`

### Task 1: Add Keycloak Runtime and Realm Bootstrap

**Files:**

- Modify: `docker-compose.yml`
- Create: `docs/keycloak/realm-export/libreroo-realm.json`

- [ ] **Step 1: Capture baseline compose services**

Run: `docker compose config --services`  
Expected: `postgres` present; `keycloak` not present yet.

- [ ] **Step 2: Add Keycloak service and realm import**

```yaml
keycloak:
  image: quay.io/keycloak/keycloak:26.1
  container_name: libreroo-keycloak
  command: start-dev --import-realm
  environment:
    KEYCLOAK_ADMIN: ${KEYCLOAK_ADMIN}
    KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
    KC_HTTP_PORT: 8080
  ports:
    - "8080:8080"
  volumes:
    - ./docs/keycloak/realm-export:/opt/keycloak/data/import
  depends_on:
    - postgres
```

- [ ] **Step 3: Add local realm export with roles and SPA/public API clients**

```json
{
  "realm": "libreroo",
  "enabled": true,
  "roles": { "realm": [{ "name": "member" }, { "name": "librarian" }, { "name": "admin" }] },
  "clients": [{ "clientId": "libreroo-web" }, { "clientId": "libreroo-api" }]
}
```

- [ ] **Step 4: Validate runtime**

Run: `docker compose up -d postgres keycloak`  
Expected: both containers running; Keycloak discovery endpoint reachable.

### Task 2: Add API Auth Configuration and Environment Guardrails

**Files:**

- Modify: `apps/libreroo-api/appsettings.json`
- Modify: `apps/libreroo-api/appsettings.Development.json`
- Modify: `apps/libreroo-api/Program.cs`

- [ ] **Step 1: Add failing config test/read assertion (optional fast check)**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: fails once auth-required endpoints are introduced (pre-change baseline can be green).

- [ ] **Step 2: Add typed auth config with environment-aware HTTPS metadata**

```json
"Auth": {
  "Authority": "http://localhost:8080/realms/libreroo",
  "Audience": "libreroo-api",
  "RequireHttpsMetadata": false
}
```

```csharp
var authSection = builder.Configuration.GetSection("Auth");
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = authSection["Authority"];
        options.Audience = authSection["Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsDevelopment()
            ? bool.Parse(authSection["RequireHttpsMetadata"] ?? "false")
            : true;
    });
```

- [ ] **Step 3: Add middleware order**

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

- [ ] **Step 4: Verify build**

Run: `dotnet build apps/libreroo-api/Libreroo.Api.csproj -v minimal`  
Expected: BUILD SUCCEEDED.

### Task 3: Implement Access Domain, Persistence, and Bootstrap Seed

**Files:**

- Create: `apps/libreroo-api/Modules/Access/Domain/AccessRole.cs`
- Create: `apps/libreroo-api/Modules/Access/Domain/AccessUser.cs`
- Create: `apps/libreroo-api/Modules/Access/Domain/AccessRoleAssignment.cs`
- Create: `apps/libreroo-api/Modules/Access/Infrastructure/Configurations/AccessUserConfiguration.cs`
- Create: `apps/libreroo-api/Modules/Access/Infrastructure/Configurations/AccessRoleAssignmentConfiguration.cs`
- Modify: `apps/libreroo-api/Shared/Infrastructure/Persistence/LibrerooDbContext.cs`
- Create: `apps/libreroo-api/Shared/Infrastructure/Bootstrap/AccessBootstrapOptions.cs`
- Create: `apps/libreroo-api/Shared/Infrastructure/Bootstrap/AccessBootstrapper.cs`

- [ ] **Step 1: Write failing domain tests for role and linkage checks**

```csharp
[Fact]
public void HasRole_WhenAssignmentMissing_ReturnsFalse()
{
    var user = AccessUser.Create("sub-123", "member@demo.local", "Member User");
    Assert.False(user.HasRole(AccessRole.Member));
}
```

- [ ] **Step 2: Run domain tests and verify failure**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: FAIL because `Access*` types do not exist.

- [ ] **Step 3: Implement entities and mappings**

```csharp
public enum AccessRole { Member = 1, Librarian = 2, Admin = 3 }

public sealed class AccessUser
{
    public Guid Id { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public int? MemberId { get; private set; }
    public List<AccessRoleAssignment> RoleAssignments { get; } = [];
}
```

- [ ] **Step 4: Add bootstrap seed for first admin (env/config driven)**

```csharp
public sealed class AccessBootstrapOptions
{
    public bool Enabled { get; init; }
    public string? AdminSubject { get; init; }
    public string? AdminEmail { get; init; }
}
```

- [ ] **Step 5: Add migration and verify**

Run: `dotnet ef migrations add AddAccessModule --project apps/libreroo-api/Libreroo.Api.csproj --startup-project apps/libreroo-api/Libreroo.Api.csproj`  
Expected: migration adds `AccessUsers` and `AccessRoleAssignments`.

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: PASS.

### Task 4: Implement DB-Authoritative Authorization Policies

**Files:**

- Create: `apps/libreroo-api/Shared/Security/AccessPolicies.cs`
- Create: `apps/libreroo-api/Shared/Security/AccessPolicyRequirements.cs`
- Create: `apps/libreroo-api/Shared/Security/AccessPolicyHandlers.cs`
- Modify: `apps/libreroo-api/Program.cs`
- Modify: `tests/Libreroo.Api.Tests/CustomWebApplicationFactory.cs`
- Create: `tests/Libreroo.Api.Tests/Auth/AuthPolicyTests.cs`

- [ ] **Step 1: Write failing `401/403` policy matrix tests**

```csharp
[Fact]
public async Task GetMembers_WithMemberRoleInAccessDb_ReturnsForbidden()
{
    var client = CreateClient("member");
    var response = await client.GetAsync("/members");
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

- [ ] **Step 2: Add policies using requirements, not `RequireRole(...)`**

```csharp
options.AddPolicy(AccessPolicies.MemberOrAbove, p => p.Requirements.Add(new MinimumRoleRequirement(AccessRole.Member)));
options.AddPolicy(AccessPolicies.LibrarianOrAdmin, p => p.Requirements.Add(new MinimumRoleRequirement(AccessRole.Librarian)));
options.AddPolicy(AccessPolicies.AdminOnly, p => p.Requirements.Add(new MinimumRoleRequirement(AccessRole.Admin)));
```

- [ ] **Step 3: Implement handlers resolving roles from `AccessService` by token `sub`**

```csharp
var subject = context.User.FindFirst("sub")?.Value;
var user = await accessService.GetCurrentUserAsync(subject!, cancellationToken);
if (user.HasRole(requiredRole)) { context.Succeed(requirement); }
```

- [ ] **Step 4: Verify policy tests**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS for `401/403` matrix with DB-backed role decisions.

### Task 5: Add Current User Resolution and Provisioning Rules

**Files:**

- Create: `apps/libreroo-api/Modules/Access/Application/IAccessService.cs`
- Create: `apps/libreroo-api/Modules/Access/Application/AccessService.cs`
- Create: `apps/libreroo-api/Modules/Access/Application/CurrentUserContext.cs`
- Modify: `apps/libreroo-api/Program.cs`
- Create: `tests/Libreroo.Domain.Tests/Access/AccessServiceTests.cs`

- [ ] **Step 1: Add failing tests for subject resolution and unprovisioned flow**

```csharp
[Fact]
public async Task GetCurrentUser_WhenMissingSubject_ThrowsForbiddenRule()
{
    await Assert.ThrowsAsync<DomainRuleViolationException>(() => _service.GetCurrentUserAsync("missing-sub", default));
}
```

- [ ] **Step 2: Implement service contract**

```csharp
Task<AccessUser> GetCurrentUserAsync(string subject, CancellationToken ct);
Task<AccessUser> EnsureUserProfileAsync(string subject, string? email, string? displayName, CancellationToken ct);
```

- [ ] **Step 3: Implement provisioning policy**

```csharp
// Ensure profile exists, but do not grant any roles automatically.
// Authorization remains denied until admin assigns roles.
```

- [ ] **Step 4: Verify tests**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: PASS.

### Task 6: Enforce Endpoint Policies and Self-Scope Rules

**Files:**

- Modify: `apps/libreroo-api/Modules/Catalog/Api/BooksController.cs`
- Modify: `apps/libreroo-api/Modules/Catalog/Api/AuthorsController.cs`
- Modify: `apps/libreroo-api/Modules/Members/Api/MembersController.cs`
- Modify: `apps/libreroo-api/Modules/Loans/Api/LoansController.cs`
- Modify: `apps/libreroo-api/Modules/Loans/Application/{BorrowBookCommand.cs,LoanService.cs}`
- Create: `tests/Libreroo.Api.Tests/Auth/LoanAuthorizationTests.cs`

- [ ] **Step 1: Add failing ownership tests**

```csharp
[Fact]
public async Task Member_CannotBorrowOnBehalfOfAnotherMember()
{
    var response = await _memberClient.PostAsJsonAsync("/loans", new { bookId = 1, memberId = 999, borrowDateUtc = DateTime.UtcNow });
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

- [ ] **Step 2: Apply authorization policies to controllers**

```csharp
[Authorize(Policy = AccessPolicies.MemberOrAbove)]
[Authorize(Policy = AccessPolicies.LibrarianOrAdmin)]
[Authorize(Policy = AccessPolicies.AdminOnly)]
```

- [ ] **Step 3: Implement self-scope contract**

```csharp
// member: ignore/reject explicit memberId, infer from AccessUser.MemberId
// librarian/admin: may use explicit memberId for on-behalf borrow
// member return: enforce loan.MemberId == currentUser.MemberId
```

- [ ] **Step 4: Add `/loans/me/active` endpoint and behavior tests**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS for member self-scope and librarian/admin elevated scope.

### Task 7: Add Admin Access Management Endpoints

**Files:**

- Create: `apps/libreroo-api/Modules/Access/Api/AccessUsersController.cs`
- Create: `apps/libreroo-api/Modules/Access/Api/AccessDtos.cs`
- Modify: `apps/libreroo-api/Modules/Access/Application/AccessService.cs`
- Create: `tests/Libreroo.Api.Tests/Auth/AccessAdminEndpointsTests.cs`

- [ ] **Step 1: Add failing tests for non-admin denial**

```csharp
[Fact]
public async Task AssignRole_WithLibrarianRole_ReturnsForbidden()
{
    var response = await _librarianClient.PostAsJsonAsync("/access/users/roles", new { userId = Guid.NewGuid(), role = "admin" });
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

- [ ] **Step 2: Implement admin-only endpoints**

```csharp
[Authorize(Policy = AccessPolicies.AdminOnly)]
[HttpPost("roles")]
public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request, CancellationToken ct)
```

- [ ] **Step 3: Include member linkage endpoints**

```csharp
[HttpPost("{userId:guid}/member-link")]
public async Task<IActionResult> LinkMember(Guid userId, [FromBody] LinkMemberRequest request, CancellationToken ct)
```

- [ ] **Step 4: Verify tests**

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS for admin-only access management.

### Task 8: Frontend OIDC Session and Route/Role Guards

**Files:**

- Create: `apps/libreroo-web/src/app/core/auth/auth.service.ts`
- Create: `apps/libreroo-web/src/app/core/auth/auth.guard.ts`
- Create: `apps/libreroo-web/src/app/core/auth/role.guard.ts`
- Create: `apps/libreroo-web/src/app/core/auth/auth.interceptor.ts`
- Modify: `apps/libreroo-web/src/app/app.config.ts`
- Modify: `apps/libreroo-web/src/app/app.routes.ts`
- Modify: `apps/libreroo-web/src/app/app.html`

- [ ] **Step 1: Add failing route guard tests**

```ts
it('blocks protected routes when unauthenticated', () => {
  expect(guard.canActivate(route, state)).toBeFalse();
});
```

- [ ] **Step 2: Implement auth service + role state from API-backed user context**

```ts
readonly isAuthenticated = signal(false);
readonly roles = signal<string[]>([]);
hasRole(role: string): boolean { return this.roles().includes(role); }
```

- [ ] **Step 3: Remove localStorage member-selection as identity source**

```ts
// MemberContextService no longer defines who is authenticated.
// Identity comes from OIDC session + backend current user.
```

- [ ] **Step 4: Verify frontend unit tests**

Run: `npm run test -- --watch=false --browsers=ChromeHeadless` (in `apps/libreroo-web`)  
Expected: PASS for auth and route guard tests.

### Task 9: Frontend Role-Aware Loan/Member/Access Flows

**Files:**

- Modify: `apps/libreroo-web/src/app/core/services/loans-api.service.ts`
- Modify: `apps/libreroo-web/src/app/features/loans/loans-page.component.ts`
- Modify: `apps/libreroo-web/src/app/features/member/member-page.component.ts`
- Modify: `apps/libreroo-web/src/app/features/catalog/catalog-page.component.ts`
- Create: `apps/libreroo-web/src/app/features/access/access-page.component.{ts,html,scss,spec.ts}`

- [ ] **Step 1: Add failing role-visibility tests**

```ts
it('shows Access navigation only for admin users', () => {
  authService.roles.set(['admin']);
  fixture.detectChanges();
  expect(screen.getByText('Access')).toBeTruthy();
});
```

- [ ] **Step 2: Switch member active loans call to self-scope endpoint**

```ts
getMyActive(): Observable<Loan[]> {
  return this.http.get<Loan[]>(this.resolveUrl('/loans/me/active'));
}
```

- [ ] **Step 3: Enforce role-based UI behavior**

```ts
readonly canManageMembers = computed(() => this.auth.hasRole('librarian') || this.auth.hasRole('admin'));
readonly canManageAccess = computed(() => this.auth.hasRole('admin'));
```

- [ ] **Step 4: Verify frontend feature tests**

Run: `npm run test -- --watch=false --browsers=ChromeHeadless` (in `apps/libreroo-web`)  
Expected: PASS for role-aware flows.

### Task 10: Verification, Real JWT Validation, and Docs Sync

**Files:**

- Create: `tests/Libreroo.Api.Tests/Auth/KeycloakJwtValidationTests.cs`
- Modify: `README.md`
- Modify: `docs/architecture.md`
- Modify: `docs/techstack.md`
- Modify: `docs/adr/0003-phase-2-auth-with-keycloak-and-postgresql-authorization.md` (if implementation clarifies details)

- [ ] **Step 1: Add real JWT validation integration test path**

```csharp
[Fact]
public async Task ApiAcceptsValidKeycloakJwt_AndRejectsWrongAudience()
{
    // valid token -> 2xx or policy-based 403; invalid audience -> 401
}
```

- [ ] **Step 2: Run backend verification suite**

Run: `dotnet test tests/Libreroo.Domain.Tests/Libreroo.Domain.Tests.csproj -v minimal`  
Expected: PASS.

Run: `dotnet test tests/Libreroo.Api.Tests/Libreroo.Api.Tests.csproj -v minimal`  
Expected: PASS, including keycloak JWT validation tests.

- [ ] **Step 3: Run frontend verification**

Run: `npm run test -- --watch=false --browsers=ChromeHeadless` (in `apps/libreroo-web`)  
Expected: PASS.

- [ ] **Step 4: Run build verification**

Run: `dotnet build Libreroo.sln -v minimal`  
Expected: BUILD SUCCEEDED.

## Self-Review

### Spec coverage check

- DB-authoritative authorization source: Tasks 4, 5, 6.
- Provisioning and first-admin bootstrap: Tasks 3 and 5.
- Member self-scope borrow/return semantics: Task 6.
- Real Keycloak JWT validation (issuer/audience/signature path): Task 10.
- Frontend shift from local member identity to OIDC session: Tasks 8 and 9.

No uncovered spec requirement found.

### Placeholder scan

- No `TBD`/`TODO` placeholders.
- Every task includes concrete files, commands, and expected outcomes.

### Type/signature consistency

- Role names remain aligned end-to-end: `member`, `librarian`, `admin`.
- Identity key remains `sub` in all backend auth flows.
- Self-scope loan endpoint remains `/loans/me/active`.
