using CleanArchitecture.Template.Application.Products.DTOs;
using CleanArchitecture.Template.Application.Products.Mappings;
using CleanArchitecture.Template.Domain.Exceptions;
using CleanArchitecture.Template.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        product.UpdateDetails(request.Name, request.Description, request.Price);

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }
}
