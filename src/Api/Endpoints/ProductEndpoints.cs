using CleanArchitecture.Template.Application.Products.Commands.CreateProduct;
using CleanArchitecture.Template.Application.Products.Commands.DeleteProduct;
using CleanArchitecture.Template.Application.Products.Commands.UpdateProduct;
using CleanArchitecture.Template.Application.Products.Queries.GetProductById;
using CleanArchitecture.Template.Application.Products.Queries.GetProducts;
using MediatR;

namespace CleanArchitecture.Template.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for the Products feature. Each endpoint does exactly
/// one thing: translate an HTTP request into a MediatR request, and translate
/// the result back into an HTTP response. No business logic lives here — that
/// is the whole point of routing everything through Application's handlers.
/// </summary>
public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (ISender sender, bool activeOnly = false, CancellationToken cancellationToken = default) =>
            {
                var products = await sender.Send(new GetProductsQuery(activeOnly), cancellationToken);
                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .WithSummary("List all products, optionally filtered to active ones only.")
            .Produces<IReadOnlyList<Application.Products.DTOs.ProductDto>>();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var product = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
                return Results.Ok(product);
            })
            .WithName("GetProductById")
            .WithSummary("Get a single product by id.")
            .Produces<Application.Products.DTOs.ProductDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateProductCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var product = await sender.Send(command, cancellationToken);
                return Results.Created($"/api/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .WithSummary("Create a new product.")
            .Produces<Application.Products.DTOs.ProductDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new UpdateProductCommand(id, request.Name, request.Description, request.Price);
                var product = await sender.Send(command, cancellationToken);
                return Results.Ok(product);
            })
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product's details.")
            .Produces<Application.Products.DTOs.ProductDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                await sender.Send(new DeleteProductCommand(id), cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithSummary("Delete a product.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// The route already carries the id, so the request body only needs the
    /// fields that can actually change.
    /// </summary>
    public record UpdateProductRequest(string Name, string Description, decimal Price);
}
