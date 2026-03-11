namespace Basket.API.Basket.GetBasket;

// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// when requesting a shopping basket.
//
// Cart → Contains the ShoppingCart object for the user.
// The ShoppingCart typically includes:
// • UserName
// • List of basket items
// • Product details
// • Quantity and price information
//
// Example response:
//
// {
//   "cart": {
//      "userName": "muneer",
//      "items": [
//         {
//           "productId": "...",
//           "productName": "IPhone X",
//           "quantity": 1,
//           "price": 950
//         }
//      ]
//   }
// }
public record GetBasketResponse(ShoppingCart Cart);


// ------------------------------------------------------------
// Carter Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule which is part of the
// Carter library.
//
// Carter allows organizing Minimal API endpoints into
// modular classes instead of placing all endpoints
// inside Program.cs.
//
// This module defines endpoints related to retrieving
// a user's shopping basket.
public class GetBasketEndpoints : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method registers routes during application startup.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map GET /basket/{userName}
        // --------------------------------------------------------
        // This endpoint retrieves the basket for a specific user.
        //
        // Route parameter:
        // userName → identifier used to locate the user's basket
        //
        // Example request:
        // GET /basket/muneer
        app.MapGet("/basket/{userName}",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // userName → extracted from the URL route
            // sender   → MediatR ISender used to dispatch queries
            async (string userName, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Send Query through MediatR
            // ----------------------------------------------------
            // Create and send GetBasketQuery.
            //
            // MediatR automatically finds and executes
            // the corresponding handler:
            // GetBasketQueryHandler.
            var result = await sender.Send(new GetBasketQuery(userName));


            // ----------------------------------------------------
            // Step 2: Map Query Result → Response DTO
            // ----------------------------------------------------
            // Mapster converts the application layer result
            // into the API response model.
            var respose = result.Adapt<GetBasketResponse>();


            // ----------------------------------------------------
            // Step 3: Return HTTP 200 OK
            // ----------------------------------------------------
            return Results.Ok(respose);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique endpoint name
        .WithName("GetProductById")

        // Successful response type
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)

        // Possible error response
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Short description used in Swagger UI
        .WithSummary("Get Product By Id")

        // Detailed description used in Swagger documentation
        .WithDescription("Get Product By Id");
    }
}
