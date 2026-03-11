namespace Catalog.API.Products.UpdateProduct;

// ------------------------------------------------------------
// Request DTO
// ------------------------------------------------------------
// This record represents the incoming HTTP request body
// for updating an existing product.
//
// The client must provide the complete product information,
// including the Id of the product to update.
//
// Example request body:
//
// {
//   "id": "e17b1c64-3e8a-4a65-8f5c-2b7d41c0f7a1",
//   "name": "iPhone 15 Pro",
//   "category": ["Electronics", "Mobile"],
//   "description": "Updated Apple smartphone",
//   "imageFile": "iphone15pro.png",
//   "price": 1400
// }
//
// This DTO exists to keep the API contract separate from
// application layer commands.
public record UpdateProductRequest(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
);


// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned after
// successfully updating a product.
//
// IsSuccess → indicates whether the update operation
// completed successfully.
public record UpdateProductResponse(bool IsSuccess);


// ------------------------------------------------------------
// Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule.
//
// Carter allows grouping Minimal API routes into modules,
// improving code organization and keeping Program.cs clean.
//
// This module contains the endpoint responsible for
// updating product information.
public class UpdateProductEndpoint : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method is executed during application startup
    // and registers routes into the ASP.NET Core pipeline.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map PUT /products
        // --------------------------------------------------------
        // This endpoint updates an existing product.
        //
        // HTTP Method: PUT
        // Endpoint: /products
        //
        // The request body must contain the product details.
        app.MapPut("/products",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // request -> incoming HTTP request body
            // sender  -> MediatR ISender used to dispatch commands
            async (UpdateProductRequest request, ISender sender) =>
            {
                // ------------------------------------------------
                // Step 1: Convert Request DTO → Command
                // ------------------------------------------------
                // Mapster maps the API request object into
                // UpdateProductCommand used by the application layer.
                var command = request.Adapt<UpdateProductCommand>();


                // ------------------------------------------------
                // Step 2: Send Command through MediatR
                // ------------------------------------------------
                // MediatR will locate the corresponding handler:
                //
                // UpdateProductCommandHandler
                //
                // The handler performs the update operation.
                var result = await sender.Send(command);


                // ------------------------------------------------
                // Step 3: Convert Command Result → Response DTO
                // ------------------------------------------------
                // Mapster maps the application layer result
                // into a response object returned to the client.
                var response = result.Adapt<UpdateProductResponse>();


                // ------------------------------------------------
                // Step 4: Return HTTP 200 OK
                // ------------------------------------------------
                return Results.Ok(response);
            })

            // ----------------------------------------------------
            // Endpoint Metadata (Swagger / OpenAPI)
            // ----------------------------------------------------

            // Unique name for this endpoint
            .WithName("UpdateProduct")

            // Successful response type
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)

            // Validation errors
            .ProducesProblem(StatusCodes.Status400BadRequest)

            // Product not found scenario
            .ProducesProblem(StatusCodes.Status404NotFound)

            // Short summary displayed in Swagger
            .WithSummary("Update Product")

            // Detailed description for Swagger documentation
            .WithDescription("Update Product");
    }
}
