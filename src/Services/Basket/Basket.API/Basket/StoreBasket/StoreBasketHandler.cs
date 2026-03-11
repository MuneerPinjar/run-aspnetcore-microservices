using Discount.Grpc;

namespace Basket.API.Basket.StoreBasket;

// ------------------------------------------------------------
// Command Definition
// ------------------------------------------------------------
// In CQRS architecture, a Command represents an action
// that modifies application state.
//
// StoreBasketCommand is used to save or update a user's
// shopping basket.
//
// Parameter:
//
// Cart → ShoppingCart object containing the user's
// basket items and details.
//
// The command implements ICommand<StoreBasketResult>,
// meaning its handler will return a StoreBasketResult.
public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;


// ------------------------------------------------------------
// Command Result
// ------------------------------------------------------------
// This record represents the result returned after
// successfully storing the basket.
//
// UserName → Identifier of the user whose basket
// was saved.
public record StoreBasketResult(string UserName);


// ------------------------------------------------------------
// Command Validator
// ------------------------------------------------------------
// FluentValidation is used to validate incoming commands
// before they reach the handler.
//
// Validation runs automatically through the MediatR
// ValidationBehavior pipeline.
public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        // Ensure the shopping cart object is not null
        RuleFor(x => x.Cart)
            .NotNull()
            .WithMessage("Cart can not be null");

        // Ensure the cart contains a valid user name
        RuleFor(x => x.Cart.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
    }
}


// ------------------------------------------------------------
// Command Handler
// ------------------------------------------------------------
// This handler processes StoreBasketCommand.
//
// Responsibilities:
// 1. Retrieve discounts from Discount gRPC service
// 2. Apply discounts to basket items
// 3. Store the updated basket in the repository
// 4. Return the operation result
//
// Dependencies:
//
// IBasketRepository → Handles data persistence (Redis)
// DiscountProtoServiceClient → gRPC client used to call
// the Discount microservice
public class StoreBasketCommandHandler
    (IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto)

    // Implements ICommandHandler linking command to response
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(
        StoreBasketCommand command,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Apply Discounts to Basket Items
        // --------------------------------------------------------
        // Calls the Discount gRPC service to retrieve discounts
        // for each product in the basket.
        await DeductDiscount(command.Cart, cancellationToken);
        

        // --------------------------------------------------------
        // Step 2: Store Basket in Repository
        // --------------------------------------------------------
        // The repository saves the updated basket
        // in Redis distributed cache.
        await repository.StoreBasket(command.Cart, cancellationToken);


        // --------------------------------------------------------
        // Step 3: Return Result
        // --------------------------------------------------------
        return new StoreBasketResult(command.Cart.UserName);
    }


    // ------------------------------------------------------------
    // DeductDiscount Method
    // ------------------------------------------------------------
    // This method communicates with the Discount gRPC service
    // to retrieve discount values for products in the basket.
    //
    // It then adjusts the price of each item accordingly.
    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Iterate Through Basket Items
        // --------------------------------------------------------
        foreach (var item in cart.Items)
        {
            // ----------------------------------------------------
            // Call Discount gRPC Service
            // ----------------------------------------------------
            // Sends the product name to the Discount service
            // and retrieves the applicable coupon.
            var coupon = await discountProto.GetDiscountAsync(
                new GetDiscountRequest
                {
                    ProductName = item.ProductName
                },
                cancellationToken: cancellationToken
            );

            // ----------------------------------------------------
            // Apply Discount
            // ----------------------------------------------------
            // Reduce the item's price based on the discount amount.
            item.Price -= coupon.Amount;
        }
    }
}
