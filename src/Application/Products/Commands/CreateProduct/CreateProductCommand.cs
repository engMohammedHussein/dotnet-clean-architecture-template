using CleanArchitecture.Template.Application.Products.DTOs;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Sku,
    string Description,
    decimal Price,
    int StockQuantity) : IRequest<ProductDto>;
