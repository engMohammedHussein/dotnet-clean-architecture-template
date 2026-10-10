using CleanArchitecture.Template.Domain.Interfaces;
using CleanArchitecture.Template.Infrastructure.Persistence;
using CleanArchitecture.Template.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Template.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Demo mode (the default — no configuration needed) runs entirely on EF
    /// Core's InMemory provider: clone the repo, run it, it works. Set
    /// ConnectionStrings:Default in configuration to switch to a real SQLite
    /// file on disk — nothing in Application or Api changes either way. That
    /// swap is this project's equivalent of the Flutter template's
    /// _useFakeBackend flag.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseInMemoryDatabase("CleanArchitectureTemplateDemo");
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
