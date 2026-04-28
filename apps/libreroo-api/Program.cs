using Libreroo.Api.Modules.Catalog.Application;
using Libreroo.Api.Modules.Loans.Application;
using Libreroo.Api.Modules.Members.Application;
using Libreroo.Api.Shared.Api.ExceptionHandling;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();

app.Run();

public partial class Program;
