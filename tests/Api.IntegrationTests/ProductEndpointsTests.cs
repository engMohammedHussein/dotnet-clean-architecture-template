using System.Net;
using System.Net.Http.Json;
using CleanArchitecture.Template.Application.Products.Commands.CreateProduct;
using CleanArchitecture.Template.Application.Products.DTOs;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Template.Api.IntegrationTests;

/// <summary>
/// Exercises the full request pipeline — routing, model binding, MediatR,
/// validation, EF Core, and the exception-handling middleware — the same way
/// a real client would, over HTTP against an in-process test server.
/// </summary>
public class ProductEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_OnFreshDatabase_ReturnsSeededProducts()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products!.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task CreateProduct_ThenGetById_ReturnsTheCreatedProduct()
    {
        var command = new CreateProductCommand("Integration Test Widget", $"SKU-IT-{Guid.NewGuid():N}", "desc", 15.5m, 3);

        var createResponse = await _client.PostAsJsonAsync("/api/products", command);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/products/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        fetched!.Name.Should().Be("Integration Test Widget");
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_Returns409Conflict()
    {
        var sku = $"SKU-DUP-{Guid.NewGuid():N}";
        var first = new CreateProductCommand("First", sku, "desc", 10m, 1);
        var second = new CreateProductCommand("Second", sku, "desc", 20m, 2);

        (await _client.PostAsJsonAsync("/api/products", first)).StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicateResponse = await _client.PostAsJsonAsync("/api/products", second);

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_Returns400WithValidationErrors()
    {
        var command = new CreateProductCommand(string.Empty, string.Empty, "desc", -5m, -1);

        var response = await _client.PostAsJsonAsync("/api/products", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductById_WithUnknownId_Returns404()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_ThenGetById_Returns404()
    {
        var command = new CreateProductCommand("To Delete", $"SKU-DEL-{Guid.NewGuid():N}", "desc", 5m, 1);
        var created = await (await _client.PostAsJsonAsync("/api/products", command))
            .Content.ReadFromJsonAsync<ProductDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
