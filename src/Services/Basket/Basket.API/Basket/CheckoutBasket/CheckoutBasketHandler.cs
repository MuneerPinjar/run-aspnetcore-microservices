using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

// ------------------------------------------------------------
// Command Definition
// ------------------------------------------------------------
// In CQRS architecture, a Command represents an operation
// that modifies application state.
//
// CheckoutBasketCommand is used when a user proceeds to
// checkout their shopping basket.
//
// Parameter:
//
// BasketCheckoutDto → Contains checkout information such as:
// • UserName
// • Customer details
// • Address
// • Payment information
//
// The command implements ICommand<CheckoutBasketResult>,
// meaning its handler returns CheckoutBasketResult.
public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto) 
    : ICommand<CheckoutBasketResult>;


// ------------------------------------------------------------
// Command Result
// ------------------------------------------------------------
// This record represents the result returned after the
// checkout process completes.
//
// IsSuccess → Indicates whether the checkout operation
// completed successfully.
public record CheckoutBasketResult(bool IsSuccess);


// ------------------------------------------------------------
// Command Validator
// ------------------------------------------------------------
// FluentValidation ensures the command contains valid data
// before it reaches the handler.
//
// Validation runs automatically through the MediatR
// ValidationBehavior pipeline.
public class CheckoutBasketCommandValidator 
    : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        // Ensure BasketCheckoutDto is provided
        RuleFor(x => x.BasketCheckoutDto)
            .NotNull()
            .WithMessage("BasketCheckoutDto can't be null");

        // Ensure UserName is present
        RuleFor(x => x.BasketCheckoutDto.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
    }
}


// ------------------------------------------------------------
// Command Handler
// ------------------------------------------------------------
// This handler processes CheckoutBasketCommand.
//
// Responsibilities:
// 1. Retrieve the user's basket
// 2. Convert checkout DTO into an event message
// 3. Publish checkout event to RabbitMQ using MassTransit
// 4. Delete the basket after successful checkout
//
// Dependencies:
//
// IBasketRepository → Retrieves and deletes baskets
// IPublishEndpoint → Publishes events to RabbitMQ
public class CheckoutBasketCommandHandler
    (IBasketRepository repository, IPublishEndpoint publishEndpoint)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(
        CheckoutBasketCommand command,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Retrieve Existing Basket
        // --------------------------------------------------------
        // Fetch the user's basket from the repository.
        // This typically retrieves the basket from Redis
        // or persistent storage.
        var basket = await repository.GetBasket(
            command.BasketCheckoutDto.UserName,
            cancellationToken);

        // If basket doesn't exist, checkout cannot proceed
        if (basket == null)
        {
            return new CheckoutBasketResult(false);
        }


        // --------------------------------------------------------
        // Step 2: Convert Checkout DTO → Event Message
        // --------------------------------------------------------
        // Mapster maps BasketCheckoutDto to BasketCheckoutEvent.
        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();


        // --------------------------------------------------------
        // Step 3: Attach Basket Total Price
        // --------------------------------------------------------
        // Include total basket price in the event message.
        eventMessage.TotalPrice = basket.TotalPrice;


        // --------------------------------------------------------
        // Step 4: Publish Event to RabbitMQ
        // --------------------------------------------------------
        // MassTransit sends the event to the message broker.
        //
        // Other microservices (such as Ordering Service)
        // subscribe to this event and process it asynchronously.
        await publishEndpoint.Publish(eventMessage, cancellationToken);


        // --------------------------------------------------------
        // Step 5: Delete Basket After Checkout
        // --------------------------------------------------------
        // Once checkout is initiated, the basket is cleared.
        await repository.DeleteBasket(
            command.BasketCheckoutDto.UserName,
            cancellationToken);


        // --------------------------------------------------------
        // Step 6: Return Result
        // --------------------------------------------------------
        return new CheckoutBasketResult(true);
    }
}
