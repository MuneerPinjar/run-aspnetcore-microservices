namespace Basket.API.Basket.DeleteBasket;

// ------------------------------------------------------------
// Command Definition
// ------------------------------------------------------------
// In CQRS architecture, a Command represents an action
// that modifies the application state.
//
// DeleteBasketCommand is used to delete a user's shopping
// basket from the system.
//
// Parameter:
//
// UserName → Identifier of the user whose basket
// should be deleted.
//
// Example request:
// DELETE /basket/{username}
//
// This command implements ICommand<DeleteBasketResult>,
// meaning the handler will return a DeleteBasketResult.
public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;


// ------------------------------------------------------------
// Command Result
// ------------------------------------------------------------
// This record represents the result returned after
// attempting to delete the basket.
//
// IsSuccess → Indicates whether the delete operation
// was successful.
public record DeleteBasketResult(bool IsSuccess);


// ------------------------------------------------------------
// Command Validator
// ------------------------------------------------------------
// FluentValidation is used to validate the command before
// it reaches the handler.
//
// This validation runs automatically through the
// MediatR ValidationBehavior pipeline.
//
// If validation fails, a ValidationException will be thrown.
public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        // Ensure the username is provided
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
    }
}


// ------------------------------------------------------------
// Command Handler
// ------------------------------------------------------------
// This handler processes the DeleteBasketCommand.
//
// Responsibilities:
// 1. Receive the command through MediatR
// 2. Delete the basket using the repository
// 3. Return the operation result
//
// IBasketRepository abstracts the data access logic.
// In this service, the repository interacts with Redis
// where shopping baskets are stored.
public class DeleteBasketCommandHandler(IBasketRepository repository) 

    // Implements ICommandHandler linking command
    // with its response type.
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(
        DeleteBasketCommand command,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Delete Basket from Repository
        // --------------------------------------------------------
        // The repository removes the basket associated with
        // the specified username from Redis distributed cache.
        await repository.DeleteBasket(command.UserName, cancellationToken);


        // --------------------------------------------------------
        // Step 2: Return Result
        // --------------------------------------------------------
        // Return success status after deletion.
        return new DeleteBasketResult(true);
    }
}
