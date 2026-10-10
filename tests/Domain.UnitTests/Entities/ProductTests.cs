using CleanArchitecture.Template.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Domain.UnitTests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_SetsAllPropertiesAndDefaultsToActive()
    {
        var product = Product.Create("Wireless Mouse", "sku-1001", "  A mouse.  ", 49.90m, 10);

        product.Name.Should().Be("Wireless Mouse");
        product.Sku.Should().Be("SKU-1001", "SKUs are normalized to upper-case");
        product.Description.Should().Be("A mouse.", "the description should be trimmed");
        product.Price.Should().Be(49.90m);
        product.StockQuantity.Should().Be(10);
        product.IsActive.Should().BeTrue();
        product.Id.Should().NotBeEmpty();
        product.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_Throws(string invalidName)
    {
        var act = () => Product.Create(invalidName, "SKU-1", "desc", 10m, 1);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Create_WithNegativePrice_Throws()
    {
        var act = () => Product.Create("Mouse", "SKU-1", "desc", -1m, 1);

        act.Should().Throw<ArgumentException>().WithParameterName("price");
    }

    [Fact]
    public void Create_WithNegativeStockQuantity_Throws()
    {
        var act = () => Product.Create("Mouse", "SKU-1", "desc", 10m, -1);

        act.Should().Throw<ArgumentException>().WithParameterName("quantity");
    }

    [Fact]
    public void UpdateDetails_ChangesNameDescriptionAndPrice_AndStampsUpdatedAt()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 1);

        product.UpdateDetails("Gaming Mouse", "A better mouse.", 79.99m);

        product.Name.Should().Be("Gaming Mouse");
        product.Description.Should().Be("A better mouse.");
        product.Price.Should().Be(79.99m);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AdjustStock_WithPositiveDelta_IncreasesQuantity()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5);

        product.AdjustStock(3);

        product.StockQuantity.Should().Be(8);
    }

    [Fact]
    public void AdjustStock_WithNegativeDeltaWithinStock_DecreasesQuantity()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5);

        product.AdjustStock(-3);

        product.StockQuantity.Should().Be(2);
    }

    [Fact]
    public void AdjustStock_BelowZero_ThrowsAndLeavesQuantityUnchanged()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5);

        var act = () => product.AdjustStock(-10);

        act.Should().Throw<InvalidOperationException>();
        product.StockQuantity.Should().Be(5, "a rejected adjustment must not partially apply");
    }

    [Fact]
    public void Deactivate_ThenActivate_TogglesIsActive()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5);

        product.Deactivate();
        product.IsActive.Should().BeFalse();

        product.Activate();
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoProductsWithSameId_AreEqual()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5);

        // Simulate the same entity loaded twice (e.g. by EF Core) by comparing
        // against itself through the base-class equality override.
        var sameReference = product;

        (product == sameReference).Should().BeTrue();
        product.Equals(sameReference).Should().BeTrue();
    }
}
