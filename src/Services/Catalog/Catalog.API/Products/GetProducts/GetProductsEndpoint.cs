namespace Catalog.API.Products.GetProducts;

// ------------------------------------------------------------
// Request DTO
// ------------------------------------------------------------
// This record represents the incoming HTTP request parameters
// used to retrieve a list of products.
//
// The parameters support pagination:
//
// PageNumber → Which page of results to fetch
// PageSize   → Number of records per page
//
// Default values:
// PageNumber = 1
// PageSize = 10
//
// These values will be automatically bound from the query string.
//
// Example request:
// GET /products?pageNumber=1&pageSize=10
public record GetProductsRequest(
    int? PageNumber = 1,
    int? PageSize = 10
);


// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client.
//
// It contains a collection of Product objects retrieved
// from the database.
public record GetProductsResponse(IEnumerable<Product> Products);


// ------------------------------------------------------------
// Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule from the Carter library.
//
// Carter helps organize Minimal API endpoints into modular
// components instead of placing all routes inside Program.cs.
//
// This module handles endpoints related to retrieving products.
public class GetProductsEndpoint : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method registers the routes for this module.
    // It is automatically executed during application startup.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map GET /products Endpoint
        // --------------------------------------------------------
        app.MapGet("/products",

        // --------------------------------------------------------
        // Endpoint Handler
        // --------------------------------------------------------
        // request -> incoming query parameters
        // sender  -> MediatR ISender used to dispatch queries
        async ([AsParameters] GetProductsRequest request, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Convert Request DTO → Query
            // ----------------------------------------------------
            // Adapt() is provided by Mapster.
            // It maps the incoming API request object into
            // a CQRS query object used by the application layer.
            var query = request.Adapt<GetProductsQuery>();


            // ----------------------------------------------------
            // Step 2: Send Query via MediatR
            // ----------------------------------------------------
            // MediatR will locate the corresponding handler:
            //
            // GetProductsQueryHandler
            //
            // and execute it.
            var result = await sender.Send(query);


            // ----------------------------------------------------
            // Step 3: Convert Query Result → API Response
            // ----------------------------------------------------
            // Mapster converts the application layer result
            // into the response DTO returned to the client.
            var response = result.Adapt<GetProductsResponse>();


            // ----------------------------------------------------
            // Step 4: Return HTTP 200 OK Response
            // ----------------------------------------------------
            return Results.Ok(response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (OpenAPI / Swagger)
        // --------------------------------------------------------

        // Assigns a unique name to the endpoint
        .WithName("GetProducts")

        // Defines the successful response type
        .Produces<GetProductsResponse>(StatusCodes.Status200OK)

        // Defines possible error responses
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Short summary for API documentation
        .WithSummary("Get Products")

        // Detailed description for API documentation
        .WithDescription("Get Products");
    }
}
