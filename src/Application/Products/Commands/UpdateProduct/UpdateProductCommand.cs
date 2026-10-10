using CleanArchitecture.Template.Application.Products.DTOs;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price) : IRequest<ProductDto>;
