using CleanArchitecture.Template.Application.Products.Commands.CreateProduct;
using CleanArchitecture.Template.Application.UnitTests.Common;
using CleanArchitecture.Template.Domain.Entities;
using CleanArchitecture.Template.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Application.UnitTests.Products.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNewSku_CreatesAndPersistsTheProduct()
    {
        var repository = InMemoryProductRepository.WithProducts();
        var handler = new CreateProductCommandHandler(repository);
        var command = new CreateProductCommand("Wireless Mouse", "SKU-1001", "A mouse.", 49.90m, 10);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Wireless Mouse");
        result.Sku.Should().Be("SKU-1001");
        repository.Added.Should().HaveCount(1);
        repository.SaveChangesCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithDuplicateSku_ThrowsAndDoesNotPersist()
    {
        var existing = Product.Create("Existing", "SKU-1001", "desc", 10m, 1);
        var repository = InMemoryProductRepository.WithProducts(existing);
        var handler = new CreateProductCommandHandler(repository);
        var command = new CreateProductCommand("New Product", "sku-1001", "desc", 20m, 2);

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DuplicateSkuException>();
        repository.Added.Should().BeEmpty();
        repository.SaveChangesCalled.Should().BeFalse();
    }
}
