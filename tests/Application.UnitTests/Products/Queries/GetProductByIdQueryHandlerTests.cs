using CleanArchitecture.Template.Application.Products.Queries.GetProductById;
using CleanArchitecture.Template.Application.UnitTests.Common;
using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Application.UnitTests.Products.Queries;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingId_ReturnsTheProduct()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 1);
        var repository = InMemoryProductRepository.WithProducts(product);
        var handler = new GetProductByIdQueryHandler(repository);

        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.Id.Should().Be(product.Id);
        result.Name.Should().Be("Mouse");
    }

    [Fact]
    public async Task Handle_WithUnknownId_ThrowsNotFound()
    {
        var repository = InMemoryProductRepository.WithProducts();
        var handler = new GetProductByIdQueryHandler(repository);

        var act = async () => await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
