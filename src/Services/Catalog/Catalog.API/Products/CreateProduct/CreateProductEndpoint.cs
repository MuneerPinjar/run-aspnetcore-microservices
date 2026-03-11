namespace Catalog.API.Products.CreateProduct;

// ------------------------------------------------------------
// Request DTO
// ------------------------------------------------------------
// This record represents the incoming HTTP request body
// for creating a product.
//
// It is used by the API endpoint to receive data from the client.
//
// Example JSON request:
// {
//   "name": "iPhone 15",
//   "category": ["Electronics", "Mobile"],
//   "description": "Latest Apple smartphone",
//   "imageFile": "iphone15.png",
//   "price": 1200
// }
//
// This DTO is intentionally separated from the Command
// to maintain a clear API contract and avoid tight coupling
// between the API layer and the application layer.
public record CreateProductRequest(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
);


// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// after a product is successfully created.
//
// Only the Id of the created product is returned.
// This allows the client to reference or fetch the resource.
//
// Example Response:
// {
//    "id": "c9b3f10e-9e32-4a3f-9b6a-5a4c8d0e2b6b"
// }
public record CreateProductResponse(Guid Id);


// ------------------------------------------------------------
// Endpoint Module
// ------------------------------------------------------------
// ICarterModule is part of the Carter library.
//
// Carter is a lightweight framework that organizes
// Minimal API endpoints into modules.
//
// Instead of defining all endpoints in Program.cs,
// we group related endpoints into modules.
//
// This improves:
// - Maintainability
// - Readability
// - Scalability
//
// This module handles Product creation endpoints.
public class CreateProductEndpoint : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method is automatically called during application
    // startup to register endpoints into the ASP.NET pipeline.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map POST /products endpoint
        // --------------------------------------------------------
        // This defines an HTTP POST endpoint used to create
        // a new product.
        app.MapPost("/products",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // request -> incoming HTTP request body
            // sender  -> MediatR ISender used to dispatch commands
            async (CreateProductRequest request, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Convert Request DTO → Command
            // ----------------------------------------------------
            // Adapt() is from Mapster.
            // It maps the API request object into the
            // CreateProductCommand used in the application layer.
            var command = request.Adapt<CreateProductCommand>();


            // ----------------------------------------------------
            // Step 2: Send Command via MediatR
            // ----------------------------------------------------
            // The command is sent to the corresponding handler:
            //
            // CreateProductCommandHandler
            //
            // MediatR automatically resolves and executes
            // the correct handler.
            var result = await sender.Send(command);


            // ----------------------------------------------------
            // Step 3: Convert Command Result → API Response
            // ----------------------------------------------------
            // Again using Mapster to map the internal
            // application result into the response DTO.
            var response = result.Adapt<CreateProductResponse>();


            // ----------------------------------------------------
            // Step 4: Return HTTP 201 Created
            // ----------------------------------------------------
            // Created() returns:
            // - Status code: 201
            // - Location header pointing to the new resource
            // - Response body containing the result
            return Results.Created($"/products/{response.Id}", response);

        })

        // --------------------------------------------------------
        // Endpoint Metadata (for Swagger / OpenAPI)
        // --------------------------------------------------------

        // Name of the endpoint (used internally or for linking)
        .WithName("CreateProduct")

        // Defines the successful response type
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)

        // Defines possible error response
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Short summary for API documentation
        .WithSummary("Create Product")

        // Detailed description for API documentation
        .WithDescription("Create Product");
    }
}
