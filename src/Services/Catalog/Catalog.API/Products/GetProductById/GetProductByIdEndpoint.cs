namespace Catalog.API.Products.GetProductById;

// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// when requesting a product by its Id.
//
// It contains the Product entity retrieved from the database.
//
// Example response:
//
// {
//   "product": {
//      "id": "b3f92b91-9b19-4a41-8f19-0b76a01c5d0f",
//      "name": "iPhone 15",
//      "category": ["Electronics"],
//      "description": "Latest Apple smartphone",
//      "imageFile": "iphone15.png",
//      "price": 1200
//   }
// }
public record GetProductByIdResponse(Product Product);


// ------------------------------------------------------------
// Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule.
//
// Carter allows grouping Minimal API routes into modules,
// improving code organization compared to placing all routes
// inside Program.cs.
//
// This module defines endpoints related to retrieving
// a product by its Id.
public class GetProductByIdEndpoint : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method is called during application startup
    // to register API routes into the ASP.NET Core pipeline.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map GET /products/{id}
        // --------------------------------------------------------
        // Defines an HTTP GET endpoint used to retrieve
        // a single product using its unique identifier.
        app.MapGet("/products/{id}",

        // --------------------------------------------------------
        // Endpoint Handler
        // --------------------------------------------------------
        // id     -> Route parameter extracted from URL
        // sender -> MediatR ISender used to dispatch queries
        async (Guid id, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Send Query via MediatR
            // ----------------------------------------------------
            // A GetProductByIdQuery is created using the provided Id.
            //
            // MediatR will locate the corresponding handler:
            // GetProductByIdQueryHandler
            //
            // That handler is responsible for fetching
            // the product from the database.
            var result = await sender.Send(new GetProductByIdQuery(id));


            // ----------------------------------------------------
            // Step 2: Convert Query Result → Response DTO
            // ----------------------------------------------------
            // Mapster is used to map the application layer result
            // (GetProductByIdResult) into an API response object
            // (GetProductByIdResponse).
            var response = result.Adapt<GetProductByIdResponse>();


            // ----------------------------------------------------
            // Step 3: Return HTTP 200 OK Response
            // ----------------------------------------------------
            return Results.Ok(response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (for Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique endpoint name
        .WithName("GetProductById")

        // Successful response type
        .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)

        // Possible error response
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Short description shown in Swagger
        .WithSummary("Get Product By Id")

        // Detailed description shown in Swagger
        .WithDescription("Get Product By Id");
    }
}
