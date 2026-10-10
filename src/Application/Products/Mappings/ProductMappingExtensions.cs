using CleanArchitecture.Template.Application.Products.DTOs;
using CleanArchitecture.Template.Domain.Entities;

namespace CleanArchitecture.Template.Application.Products.Mappings;

/// <summary>
/// Hand-written mapping, deliberately. A template this small doesn't need
/// AutoMapper's reflection and configuration-validation overhead — one method
/// is easier to read, debug, and refactor than a mapping profile.
/// </summary>
public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product) => new(
        product.Id,
        product.Name,
        product.Sku,
        product.Description,
        product.Price,
        product.StockQuantity,
        product.IsActive,
        product.CreatedAt,
        product.UpdatedAt);
}
