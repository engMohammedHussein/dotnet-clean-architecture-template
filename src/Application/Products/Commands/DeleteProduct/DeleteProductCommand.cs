using MediatR;

namespace CleanArchitecture.Template.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
