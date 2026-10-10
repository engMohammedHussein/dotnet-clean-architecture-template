using CleanArchitecture.Template.Domain.Exceptions;
using CleanArchitecture.Template.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
    }
}
