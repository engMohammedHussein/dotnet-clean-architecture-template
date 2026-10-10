using CleanArchitecture.Template.Application.Products.Queries.GetProducts;
using CleanArchitecture.Template.Application.UnitTests.Common;
using CleanArchitecture.Template.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Application.UnitTests.Products.Queries;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoFilter_ReturnsAllProductsOrderedByName()
    {
        var zebra = Product.Create("Zebra Cable", "SKU-2", "desc", 5m, 1);
        var apple = Product.Create("Apple Stand", "SKU-1", "desc", 5m, 1);
        var repository = InMemoryProductRepository.WithProducts(zebra, apple);
        var handler = new GetProductsQueryHandler(repository);

        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().ContainInOrder("Apple Stand", "Zebra Cable");
    }

    [Fact]
    public async Task Handle_WithActiveOnly_ExcludesDeactivatedProducts()
    {
        var active = Product.Create("Active Product", "SKU-1", "desc", 5m, 1);
        var inactive = Product.Create("Inactive Product", "SKU-2", "desc", 5m, 1);
        inactive.Deactivate();

        var repository = InMemoryProductRepository.WithProducts(active, inactive);
        var handler = new GetProductsQueryHandler(repository);

        var result = await handler.Handle(new GetProductsQuery(ActiveOnly: true), CancellationToken.None);

        result.Should().ContainSingle();
        result.Single().Name.Should().Be("Active Product");
    }
}
