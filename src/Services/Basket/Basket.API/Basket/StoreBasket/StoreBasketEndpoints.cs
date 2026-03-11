namespace Basket.API.Basket.StoreBasket;

// ------------------------------------------------------------
// Request DTO
// ------------------------------------------------------------
// This record represents the incoming HTTP request body
// used to store or update a user's shopping basket.
//
// Cart → ShoppingCart object containing:
// • UserName
// • List of basket items
// • Product details such as name, price, quantity
//
// Example request body:
//
// {
//   "cart": {
//      "userName": "muneer",
//      "items": [
//        {
//          "productId": "...",
//          "productName": "IPhone X",
//          "price": 950,
//          "quantity": 1
//        }
//      ]
//   }
// }
public record StoreBasketRequest(ShoppingCart Cart);


// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned after
// successfully storing the basket.
//
// UserName → identifies which user's basket was stored.
public record StoreBasketResponse(string UserName);


// ------------------------------------------------------------
// Carter Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule which allows grouping
// related endpoints into modular components.
//
// Instead of defining endpoints in Program.cs, Carter modules
// help keep the project organized and scalable.
//
// This module contains the endpoint responsible for storing
// or updating a shopping basket.
public class StoreBasketEndpoints : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method registers API routes when the application starts.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map POST /basket
        // --------------------------------------------------------
        // This endpoint stores or updates a user's basket.
        //
        // HTTP Method: POST
        // Route: /basket
        //
        // Example request:
        // POST /basket
        app.MapPost("/basket",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // request → HTTP request body
            // sender  → MediatR ISender used to dispatch commands
            async (StoreBasketRequest request, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Convert Request DTO → Command
            // ----------------------------------------------------
            // Mapster automatically maps StoreBasketRequest
            // into StoreBasketCommand used in the application layer.
            var command = request.Adapt<StoreBasketCommand>();


            // ----------------------------------------------------
            // Step 2: Send Command via MediatR
            // ----------------------------------------------------
            // MediatR finds and executes StoreBasketCommandHandler.
            var result = await sender.Send(command);


            // ----------------------------------------------------
            // Step 3: Convert Result → Response DTO
            // ----------------------------------------------------
            var response = result.Adapt<StoreBasketResponse>();


            // ----------------------------------------------------
            // Step 4: Return HTTP 201 Created
            // ----------------------------------------------------
            // The Location header will point to the user's basket.
            return Results.Created($"/basket/{response.UserName}", response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique endpoint name
        .WithName("CreateProduct")

        // Response when basket is successfully stored
        .Produces<StoreBasketResponse>(StatusCodes.Status201Created)

        // Possible validation errors
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Summary displayed in Swagger
        .WithSummary("Create Product")

        // Description displayed in Swagger documentation
        .WithDescription("Create Product");
    }
}
