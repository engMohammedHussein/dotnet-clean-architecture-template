using CleanArchitecture.Template.Domain.Common;

namespace CleanArchitecture.Template.Domain.Entities;

/// <summary>
/// A sellable item in the catalog. This is the one feature implemented end to
/// end in this template — swap it for whatever your real aggregate root is;
/// the layers and the wiring between them stay the same.
///
/// The constructor is private: a Product can only come into existence through
/// <see cref="Create"/>, and can only change through its own methods, so every
/// invariant below is enforced at every call site, not just the ones someone
/// remembered to validate.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    public bool IsActive { get; private set; }

    private Product()
    {
        // Required by EF Core for materialization.
    }

    public static Product Create(string name, string sku, string description, decimal price, int stockQuantity)
    {
        var product = new Product();

        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTimeOffset.UtcNow;
        product.IsActive = true;

        product.SetName(name);
        product.SetSku(sku);
        product.Description = description?.Trim() ?? string.Empty;
        product.SetPrice(price);
        product.SetStockQuantity(stockQuantity);

        return product;
    }

    public void UpdateDetails(string name, string description, decimal price)
    {
        SetName(name);
        Description = description?.Trim() ?? string.Empty;
        SetPrice(price);
        Touch();
    }

    public void AdjustStock(int delta)
    {
        var newQuantity = StockQuantity + delta;

        if (newQuantity < 0)
        {
            throw new InvalidOperationException(
                $"Cannot adjust stock for \"{Name}\" by {delta}: that would leave a negative quantity ({newQuantity}).");
        }

        StockQuantity = newQuantity;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters.", nameof(name));

        Name = name.Trim();
    }

    private void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("Product SKU cannot be empty.", nameof(sku));

        Sku = sku.Trim().ToUpperInvariant();
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Product price cannot be negative.", nameof(price));

        Price = price;
    }

    private void SetStockQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Initial stock quantity cannot be negative.", nameof(quantity));

        StockQuantity = quantity;
    }
}
