namespace Catalog.API.Products.GetProductByCategory;

// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// when requesting products by a specific category.
//
// It contains a collection of Product entities that match
// the requested category.
//
// Example response:
//
// {
//   "products": [
//     {
//       "id": "d3e7c23b-3c15-4f1b-a1f0-9c0b52b7b1f0",
//       "name": "iPhone 15",
//       "category": ["Electronics", "Mobile"],
//       "description": "Latest Apple smartphone",
//       "imageFile": "iphone15.png",
//       "price": 1200
//     }
//   ]
// }
public record GetProductByCategoryResponse(IEnumerable<Product> Products);


// ------------------------------------------------------------
// Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule from the Carter library.
//
// Carter helps organize Minimal API endpoints into modules,
// keeping Program.cs clean and making the project easier
// to maintain.
//
// This module defines endpoints related to retrieving
// products filtered by category.
public class GetProductByCategoryEndpoint : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method registers the routes for this module
    // into the ASP.NET Core routing pipeline during startup.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map GET /products/category/{category}
        // --------------------------------------------------------
        // This endpoint retrieves all products that belong
        // to a specific category.
        //
        // Example request:
        // GET /products/category/Electronics
        app.MapGet("/products/category/{category}",

        // --------------------------------------------------------
        // Endpoint Handler
        // --------------------------------------------------------
        // category -> Route parameter extracted from the URL
        // sender   -> MediatR ISender used to dispatch queries
        async (string category, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Send Query through MediatR
            // ----------------------------------------------------
            // Create and send the GetProductByCategoryQuery.
            //
            // MediatR will automatically locate the corresponding
            // handler: GetProductByCategoryQueryHandler.
            var result = await sender.Send(new GetProductByCategoryQuery(category));


            // ----------------------------------------------------
            // Step 2: Convert Result → Response DTO
            // ----------------------------------------------------
            // Mapster is used to map the application layer result
            // into the response object returned to the client.
            var response = result.Adapt<GetProductByCategoryResponse>();


            // ----------------------------------------------------
            // Step 3: Return HTTP 200 OK Response
            // ----------------------------------------------------
            return Results.Ok(response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique name for the endpoint
        .WithName("GetProductByCategory")

        // Defines the successful response type
        .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)

        // Defines possible error response
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Short description shown in Swagger UI
        .WithSummary("Get Product By Category")

        // Detailed description shown in Swagger UI
        .WithDescription("Get Product By Category");
    }
}
