using MediatR;

namespace BuildingBlocks.CQRS;

// ------------------------------------------------------------
// ICommand (Non-Generic Version)
// ------------------------------------------------------------
// This interface represents a CQRS Command that does NOT
// explicitly return a custom response type.
//
// It inherits from ICommand<Unit> meaning the response type
// is MediatR's Unit.
//
// Unit in MediatR is equivalent to "void" but still allows
// the pipeline to work with a consistent generic type.
//
// Example use case:
// A command like DeleteProductCommand may not need to return
// anything meaningful, so it can simply implement ICommand.
//
// Example:
// public record DeleteProductCommand(Guid Id) : ICommand;
//
// Internally it will return Unit.Value.
public interface ICommand : ICommand<Unit>
{
}


// ------------------------------------------------------------
// ICommand<TResponse> (Generic Version)
// ------------------------------------------------------------
// This interface represents a CQRS Command that RETURNS
// a response after execution.
//
// TResponse = type of result returned by the command handler.
//
// This interface extends MediatR's IRequest<TResponse>,
// which allows MediatR to route the command to the
// appropriate handler.
//
// Flow with MediatR:
// Controller/API → Send(Command) → Handler → Response
//
// Example:
// public record CreateProductCommand(...) 
//     : ICommand<CreateProductResult>;
//
// Handler:
// public class CreateProductCommandHandler
//     : ICommandHandler<CreateProductCommand, CreateProductResult>
//
// TResponse here would be CreateProductResult.
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
