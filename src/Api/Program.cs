using CleanArchitecture.Template.Api.Endpoints;
using CleanArchitecture.Template.Api.Middleware;
using CleanArchitecture.Template.Application;
using CleanArchitecture.Template.Infrastructure;
using CleanArchitecture.Template.Infrastructure.Persistence;
using CleanArchitecture.Template.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Clean Architecture Template API",
        Version = "v1",
        Description = "A reference ASP.NET Core Minimal API demonstrating Clean Architecture, CQRS with MediatR, and FluentValidation — runs out of the box against an in-memory database.",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapProductEndpoints();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    SeedData.EnsureSeeded(context);
}

app.Run();

/// <summary>
/// Exposes the implicit Program class to WebApplicationFactory&lt;Program&gt;
/// in the integration test project.
/// </summary>
public partial class Program
{
}
