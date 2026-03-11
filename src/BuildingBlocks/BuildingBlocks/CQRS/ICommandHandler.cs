using MediatR;

namespace BuildingBlocks.CQRS;

// ------------------------------------------------------------
// ICommandHandler<TCommand> (Non-Response Command Handler)
// ------------------------------------------------------------
// This interface handles commands that DO NOT return a custom
// response object.
//
// Instead of returning a custom type, it returns MediatR's
// Unit type (which represents "void").
//
// Example command:
// public record DeleteProductCommand(Guid Id) : ICommand;
//
// Since DeleteProductCommand does not return any meaningful
// data, the handler will return Unit.
//
// This interface internally inherits from:
// ICommandHandler<TCommand, Unit>
//
// Meaning the response type is fixed to Unit.
//
// Generic variance:
// "in TCommand"
// - Indicates contravariance
// - Allows the handler to accept base types of the command
public interface ICommandHandler<in TCommand>
    : ICommandHandler<TCommand, Unit>

    // Constraint ensures that the command handled by this
    // handler must implement ICommand<Unit>.
    where TCommand : ICommand<Unit>
{
}


// ------------------------------------------------------------
// ICommandHandler<TCommand, TResponse>
// ------------------------------------------------------------
// This interface represents a Command Handler that processes
// commands AND returns a response.
//
// It extends MediatR's IRequestHandler, which is responsible
// for routing requests to handlers.
//
// IRequestHandler<TRequest, TResponse> requires implementing:
//
// Task<TResponse> Handle(TRequest request, CancellationToken)
//
// Generic parameters:
// TCommand  -> command being executed
// TResponse -> result returned after command execution
//
// Example:
// public record CreateProductCommand(...) 
//     : ICommand<CreateProductResult>;
//
// Handler:
// public class CreateProductCommandHandler
//     : ICommandHandler<CreateProductCommand, CreateProductResult>
public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>

    // ------------------------------------------------------------
    // Constraints
    // ------------------------------------------------------------

    // Ensures that TCommand must be a valid CQRS command
    // returning TResponse.
    where TCommand : ICommand<TResponse>

    // Ensures the response cannot be null.
    // This helps enforce safer API contracts.
    where TResponse : notnull
{
}
