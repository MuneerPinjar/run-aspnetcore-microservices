namespace Basket.API.Basket.CheckoutBasket;

// ------------------------------------------------------------
// Request DTO
// ------------------------------------------------------------
// This record represents the HTTP request body sent by the client
// when performing a basket checkout.
//
// BasketCheckoutDto contains checkout information such as:
// • UserName
// • Customer details
// • Address information
// • Payment details
//
// Example request:
//
// {
//   "basketCheckoutDto": {
//      "userName": "muneer",
//      "firstName": "Muneer",
//      "lastName": "Pinjar",
//      "emailAddress": "muneer@example.com"
//   }
// }
public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);


// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// after the checkout process completes.
//
// IsSuccess → Indicates whether the checkout operation
// was successfully initiated.
public record CheckoutBasketResponse(bool IsSuccess);


// ------------------------------------------------------------
// Carter Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule which allows grouping
// related API endpoints together.
//
// Carter helps keep Program.cs clean by organizing endpoints
// into separate modules.
//
// This module defines the endpoint responsible for handling
// basket checkout operations.
public class CheckoutBasketEndpoints : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method is called during application startup
    // and registers API routes.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map POST /basket/checkout
        // --------------------------------------------------------
        // This endpoint initiates the basket checkout process.
        //
        // HTTP Method: POST
        // Route: /basket/checkout
        //
        // Example request:
        // POST /basket/checkout
        app.MapPost("/basket/checkout",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // request → Request body containing checkout data
            // sender  → MediatR ISender used to dispatch commands
            async (CheckoutBasketRequest request, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Convert Request DTO → Command
            // ----------------------------------------------------
            // Mapster maps CheckoutBasketRequest to
            // CheckoutBasketCommand.
            var command = request.Adapt<CheckoutBasketCommand>();


            // ----------------------------------------------------
            // Step 2: Send Command via MediatR
            // ----------------------------------------------------
            // MediatR finds the appropriate handler
            // (CheckoutBasketCommandHandler) and executes it.
            var result = await sender.Send(command);


            // ----------------------------------------------------
            // Step 3: Convert Result → Response DTO
            // ----------------------------------------------------
            var response = result.Adapt<CheckoutBasketResponse>();


            // ----------------------------------------------------
            // Step 4: Return HTTP Response
            // ----------------------------------------------------
            // Returns success status of checkout operation.
            return Results.Ok(response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique endpoint name
        .WithName("CheckoutBasket")

        // Successful response type
        .Produces<CheckoutBasketResponse>(StatusCodes.Status201Created)

        // Validation error response
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Summary shown in Swagger UI
        .WithSummary("Checkout Basket")

        // Description used in API documentation
        .WithDescription("Checkout Basket");
    }
}
