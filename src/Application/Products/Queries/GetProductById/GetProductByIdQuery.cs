using CleanArchitecture.Template.Application.Products.DTOs;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
