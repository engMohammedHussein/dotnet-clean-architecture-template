using CleanArchitecture.Template.Domain.Entities;

namespace CleanArchitecture.Template.Domain.Interfaces;

/// <summary>
/// The repository contract, owned by the domain. Infrastructure implements it;
/// Application depends only on this interface, never on EF Core or any other
/// persistence detail — the same inversion the Flutter template's
/// TodoRepository abstract class enforces between its domain and data layers.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsWithSkuAsync(string sku, Guid? excludingId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    void Update(Product product);

    void Remove(Product product);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
