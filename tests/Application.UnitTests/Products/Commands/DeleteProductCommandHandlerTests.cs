using CleanArchitecture.Template.Application.Products.Commands.DeleteProduct;
using CleanArchitecture.Template.Application.UnitTests.Common;
using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Application.UnitTests.Products.Commands;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_RemovesAndPersists()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 1);
        var repository = InMemoryProductRepository.WithProducts(product);
        var handler = new DeleteProductCommandHandler(repository);

        await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        repository.Removed.Should().ContainSingle(p => p.Id == product.Id);
        repository.SaveChangesCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithUnknownId_ThrowsNotFound()
    {
        var repository = InMemoryProductRepository.WithProducts();
        var handler = new DeleteProductCommandHandler(repository);

        var act = async () => await handler.Handle(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
