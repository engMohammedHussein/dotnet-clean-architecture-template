using CleanArchitecture.Template.Application.Products.DTOs;
using CleanArchitecture.Template.Application.Products.Mappings;
using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Exceptions;
using CleanArchitecture.Template.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _productRepository.ExistsWithSkuAsync(request.Sku, cancellationToken: cancellationToken))
        {
            throw new DuplicateSkuException(request.Sku);
        }

        var product = Product.Create(
            request.Name,
            request.Sku,
            request.Description,
            request.Price,
            request.StockQuantity);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }
}
