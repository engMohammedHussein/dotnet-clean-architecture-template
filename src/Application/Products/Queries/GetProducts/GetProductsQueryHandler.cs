using CleanArchitecture.Template.Application.Products.DTOs;
using CleanArchitecture.Template.Application.Products.Mappings;
using CleanArchitecture.Template.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Template.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);

        var filtered = request.ActiveOnly
            ? products.Where(p => p.IsActive)
            : products;

        return filtered
            .OrderBy(p => p.Name)
            .Select(p => p.ToDto())
            .ToList();
    }
}
