using CleanArchitecture.Template.Domain.Entities;

namespace CleanArchitecture.Template.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the demo database so the API is useful the moment it starts, the same
/// way the Flutter template's fake data source ships with a few Todos already
/// in it. Runs only against the InMemory/SQLite demo provider — see
/// <see cref="DependencyInjection.AddInfrastructure"/>.
/// </summary>
public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        if (context.Products.Any())
        {
            return;
        }

        var products = new[]
        {
            Product.Create("Wireless Mouse", "SKU-1001", "Ergonomic 2.4GHz wireless mouse.", 49.90m, 120),
            Product.Create("Mechanical Keyboard", "SKU-1002", "Hot-swappable mechanical keyboard, brown switches.", 249.00m, 45),
            Product.Create("USB-C Dock", "SKU-1003", "10-in-1 USB-C docking station, 100W PD passthrough.", 189.50m, 30),
            Product.Create("27\" 4K Monitor", "SKU-1004", "27-inch 4K IPS monitor, 60Hz.", 1099.00m, 15),
            Product.Create("Webcam 1080p", "SKU-1005", "1080p webcam with privacy shutter.", 99.00m, 0),
        };

        products[4].Deactivate();

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
