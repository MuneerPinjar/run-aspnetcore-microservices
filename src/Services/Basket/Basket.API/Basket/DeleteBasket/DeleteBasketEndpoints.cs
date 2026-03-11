namespace Basket.API.Basket.DeleteBasket;

// ------------------------------------------------------------
// Response DTO
// ------------------------------------------------------------
// This record represents the response returned to the client
// after attempting to delete a user's shopping basket.
//
// IsSuccess → Indicates whether the basket deletion
// operation was successful.
//
// Example response:
//
// {
//   "isSuccess": true
// }
public record DeleteBasketResponse(bool IsSuccess);


// ------------------------------------------------------------
// Carter Endpoint Module
// ------------------------------------------------------------
// This class implements ICarterModule from the Carter library.
//
// Carter allows grouping related endpoints into modules
// instead of defining all routes inside Program.cs.
//
// This module contains endpoints related to deleting
// shopping baskets.
public class DeleteBasketEndpoints : ICarterModule
{
    // ------------------------------------------------------------
    // AddRoutes Method
    // ------------------------------------------------------------
    // This method registers API routes during application startup.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // --------------------------------------------------------
        // Map DELETE /basket/{userName}
        // --------------------------------------------------------
        // This endpoint deletes the shopping basket belonging
        // to the specified user.
        //
        // HTTP Method: DELETE
        // Route: /basket/{userName}
        //
        // Example request:
        // DELETE /basket/muneer
        app.MapDelete("/basket/{userName}",

            // ----------------------------------------------------
            // Endpoint Handler
            // ----------------------------------------------------
            // userName → route parameter extracted from URL
            // sender   → MediatR ISender used to dispatch commands
            async (string userName, ISender sender) =>
        {
            // ----------------------------------------------------
            // Step 1: Send Delete Command via MediatR
            // ----------------------------------------------------
            // Create and send DeleteBasketCommand.
            //
            // MediatR automatically locates and executes
            // DeleteBasketCommandHandler.
            var result = await sender.Send(new DeleteBasketCommand(userName));


            // ----------------------------------------------------
            // Step 2: Map Command Result → Response DTO
            // ----------------------------------------------------
            // Mapster converts the application-layer result
            // into the API response model returned to the client.
            var response = result.Adapt<DeleteBasketResponse>();


            // ----------------------------------------------------
            // Step 3: Return HTTP 200 OK
            // ----------------------------------------------------
            return Results.Ok(response);
        })

        // --------------------------------------------------------
        // Endpoint Metadata (Swagger / OpenAPI)
        // --------------------------------------------------------

        // Unique name used for endpoint identification
        .WithName("DeleteProduct")

        // Successful response type
        .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)

        // Validation error response
        .ProducesProblem(StatusCodes.Status400BadRequest)

        // Resource not found response
        .ProducesProblem(StatusCodes.Status404NotFound)

        // Short summary shown in Swagger UI
        .WithSummary("Delete Product")

        // Detailed description for API documentation
        .WithDescription("Delete Product");
    }
}
