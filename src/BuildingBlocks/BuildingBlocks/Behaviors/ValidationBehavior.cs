using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors;

// ------------------------------------------------------------
// ValidationBehavior
// ------------------------------------------------------------
// This class implements MediatR's IPipelineBehavior interface.
//
// Pipeline behaviors allow executing logic BEFORE and AFTER
// a request reaches its handler.
//
// In this case, ValidationBehavior ensures that all commands
// are validated using FluentValidation BEFORE they are handled.
//
// Execution order:
//
// Request
//    │
//    ▼
// ValidationBehavior
//    │
//    ▼
// CommandHandler
//
// If validation fails, the handler will never execute.
public class ValidationBehavior<TRequest, TResponse>

    // Constructor Injection
    // All validators registered for this request type
    // are automatically injected here by Dependency Injection.
    (IEnumerable<IValidator<TRequest>> validators)

    // This class acts as a MediatR pipeline behavior
    : IPipelineBehavior<TRequest, TResponse>

    // Constraint ensures that validation runs only for
    // CQRS commands (not queries).
    where TRequest : ICommand<TResponse>
{
    // ------------------------------------------------------------
    // Handle Method
    // ------------------------------------------------------------
    // This method intercepts the request before it reaches
    // the actual command handler.
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Create FluentValidation Context
        // --------------------------------------------------------
        // Wraps the incoming request inside a validation context.
        var context = new ValidationContext<TRequest>(request);


        // --------------------------------------------------------
        // Execute All Validators
        // --------------------------------------------------------
        // If multiple validators exist for this command,
        // they are executed in parallel using Task.WhenAll.
        var validationResults =
            await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );


        // --------------------------------------------------------
        // Collect Validation Failures
        // --------------------------------------------------------
        // Extract all validation errors from the results.
        var failures =
            validationResults
                .Where(r => r.Errors.Any())      // Filter results with errors
                .SelectMany(r => r.Errors)       // Flatten all error collections
                .ToList();


        // --------------------------------------------------------
        // Throw Validation Exception if Errors Exist
        // --------------------------------------------------------
        // If any validation failures are found,
        // throw FluentValidation's ValidationException.
        //
        // This exception will be handled by the global
        // exception handler middleware.
        if (failures.Any())
            throw new ValidationException(failures);


        // --------------------------------------------------------
        // Continue Pipeline
        // --------------------------------------------------------
        // If validation succeeds, continue to the next
        // pipeline step (which is usually the command handler).
        return await next();
    }
}
