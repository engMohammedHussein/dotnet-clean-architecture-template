using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Template.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IProductRepository"/>. This is the one
/// place in the whole solution that knows EF Core exists — Application and
/// Domain never reference it, so swapping persistence providers (or writing a
/// Dapper/MongoDB implementation instead) touches only this class and its
/// registration in <see cref="DependencyInjection"/>.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Products.AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> ExistsWithSkuAsync(string sku, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();

        var query = _context.Products.Where(p => p.Sku == normalizedSku);

        if (excludingId.HasValue)
        {
            query = query.Where(p => p.Id != excludingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await _context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product) => _context.Products.Update(product);

    public void Remove(Product product) => _context.Products.Remove(product);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
