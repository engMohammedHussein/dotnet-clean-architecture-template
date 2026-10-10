using CleanArchitecture.Template.Application.Products.DTOs;
using CleanArchitecture.Template.Application.Products.Mappings;
using CleanArchitecture.Template.Domain.Exceptions;
using CleanArchitecture.Template.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        return product.ToDto();
    }
}
