using Libreroo.Api.Modules.Catalog.Application;
using Libreroo.Api.Modules.Loans.Application;
using Libreroo.Api.Modules.Members.Application;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Modules.Access.Application;
using Libreroo.Api.Shared.Api.ExceptionHandling;
using Libreroo.Api.Shared.Infrastructure.Bootstrap;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Libreroo.Api.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authSection = builder.Configuration.GetSection("Auth");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = authSection["Authority"];
        options.Audience = authSection["Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsDevelopment()
            ? bool.TryParse(authSection["RequireHttpsMetadata"], out var requireHttpsMetadata) && requireHttpsMetadata
            : true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AccessPolicies.MemberOrAbove,
        policy => policy.Requirements.Add(new MinimumRoleRequirement(AccessRole.Member)));
    options.AddPolicy(
        AccessPolicies.LibrarianOrAdmin,
        policy => policy.Requirements.Add(new MinimumRoleRequirement(AccessRole.Librarian)));
    options.AddPolicy(
        AccessPolicies.AdminOnly,
        policy => policy.Requirements.Add(new MinimumRoleRequirement(AccessRole.Admin)));
});
builder.Services.AddScoped<IAuthorizationHandler, MinimumRoleRequirementHandler>();
builder.Services.Configure<AccessBootstrapOptions>(builder.Configuration.GetSection("AccessBootstrap"));

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<LibrerooDbContext>(options =>
        options.UseInMemoryDatabase("libreroo-tests"));
}
else
{
    builder.Services.AddDbContext<LibrerooDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddScoped<LoanService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<CurrentUserContext>();
builder.Services.AddScoped<AccessBootstrapper>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();
    await dbContext.Database.MigrateAsync();

    var accessBootstrapper = scope.ServiceProvider.GetRequiredService<AccessBootstrapper>();
    await accessBootstrapper.BootstrapAsync();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
