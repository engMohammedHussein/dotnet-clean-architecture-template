namespace CleanArchitecture.Template.Application.Products.DTOs;

/// <summary>
/// The shape returned to API callers. Kept separate from the Product entity
/// so the domain model is free to change internally without breaking the
/// public contract, and so we never serialize domain methods or private
/// setters by accident.
/// </summary>
public record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    string Description,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
