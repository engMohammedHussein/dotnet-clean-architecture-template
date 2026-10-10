using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Interfaces;

namespace CleanArchitecture.Template.Application.UnitTests.Common;

/// <summary>
/// A hand-written fake implementing the same <see cref="IProductRepository"/>
/// contract the real EF Core repository implements — the same "swap the
/// implementation, not the contract" pattern the Flutter template's fake
/// remote data source demonstrates. Handler tests run against this instead of
/// a mocking library, so the tests read like ordinary code and exercise the
/// handler's real branching (found/not-found, duplicate SKU) rather than
/// a list of configured mock expectations.
/// </summary>
public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private readonly List<Product> _added = new();
    private readonly List<Product> _removed = new();

    public IReadOnlyList<Product> Added => _added;

    public IReadOnlyList<Product> Removed => _removed;

    public bool SaveChangesCalled { get; private set; }

    public static InMemoryProductRepository WithProducts(params Product[] products)
    {
        var repository = new InMemoryProductRepository();
        repository._products.AddRange(products);
        return repository;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Product>>(_products.ToList());

    public Task<bool> ExistsWithSkuAsync(string sku, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        var normalized = sku.Trim().ToUpperInvariant();

        var exists = _products.Any(p =>
            p.Sku == normalized && (!excludingId.HasValue || p.Id != excludingId.Value));

        return Task.FromResult(exists);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Add(product);
        _added.Add(product);
        return Task.CompletedTask;
    }

    public void Update(Product product)
    {
        // The fake shares the same in-memory instance, so there is nothing to
        // persist — this exists only to prove handlers call it.
    }

    public void Remove(Product product)
    {
        _products.Remove(product);
        _removed.Add(product);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalled = true;
        return Task.FromResult(1);
    }
}
