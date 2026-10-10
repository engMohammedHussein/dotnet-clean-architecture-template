using CleanArchitecture.Template.Application.Products.Commands.UpdateProduct;
using CleanArchitecture.Template.Application.UnitTests.Common;
using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Application.UnitTests.Products.Commands;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_UpdatesAndPersists()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 1);
        var repository = InMemoryProductRepository.WithProducts(product);
        var handler = new UpdateProductCommandHandler(repository);
        var command = new UpdateProductCommand(product.Id, "Gaming Mouse", "A better mouse.", 79.99m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Gaming Mouse");
        result.Price.Should().Be(79.99m);
        repository.SaveChangesCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithUnknownId_ThrowsNotFound()
    {
        var repository = InMemoryProductRepository.WithProducts();
        var handler = new UpdateProductCommandHandler(repository);
        var command = new UpdateProductCommand(Guid.NewGuid(), "Name", "desc", 1m);

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
