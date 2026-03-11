namespace Catalog.API.Products.DeleteProduct;

// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// after attempting to delete a product.
//
// IsSuccess → Indicates whether the delete operation
// was completed successfully.
//
// Example response:
//
// {
//   "isSuccess": true
// }
public record DeleteProductResponse(bool IsSuccess);


// ------------------------------------------------------------
// Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule.
//
// Carter helps organize Minimal API endpoints into modules,
// keeping the Program.cs file clean and improving
// maintainability.
//
// This module contains the endpoint responsible for
// deleting products.
public class DeleteProductEndpoint : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method is executed during application startup
    // and registers the routes defined in this module.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map DELETE /products/{id}
        // --------------------------------------------------------
        // This endpoint deletes a product based on its Id.
        //
        // HTTP Method: DELETE
        // Route: /products/{id}
        //
        // Example request:
        // DELETE /products/3c56e9f8-0c2b-4d3a-b7d1-7a7b3b90e64f
        app.MapDelete("/products/{id}",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // id     → Route parameter extracted from URL
            // sender → MediatR ISender used to dispatch commands
            async (Guid id, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Send Delete Command via MediatR
            // ----------------------------------------------------
            // Create and send DeleteProductCommand.
            //
            // MediatR automatically finds the corresponding
            // handler: DeleteProductCommandHandler.
            var result = await sender.Send(new DeleteProductCommand(id));


            // ----------------------------------------------------
            // Step 2: Convert Result → Response DTO
            // ----------------------------------------------------
            // Mapster maps the application layer result
            // into the response DTO returned to the client.
            var response = result.Adapt<DeleteProductResponse>();


            // ----------------------------------------------------
            // Step 3: Return HTTP 200 OK
            // ----------------------------------------------------
            return Results.Ok(response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique endpoint name used internally and in Swagger
        .WithName("DeleteProduct")

        // Successful response type
        .Produces<DeleteProductResponse>(StatusCodes.Status200OK)

        // Validation errors
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Product not found scenario
        .ProducesProblem(StatusCodes.Status404NotFound)

        // Short summary for Swagger UI
        .WithSummary("Delete Product")

        // Detailed description for Swagger documentation
        .WithDescription("Delete Product");
    }
}
