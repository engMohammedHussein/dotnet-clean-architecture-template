using CleanArchitecture.Template.Application.Products.DTOs;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Queries.GetProducts;

/// <summary>
/// Optional <see cref="ActiveOnly"/> filter — true returns only active products,
/// false (the default) returns everything, active or deactivated.
/// </summary>
public record GetProductsQuery(bool ActiveOnly = false) : IRequest<IReadOnlyList<ProductDto>>;
