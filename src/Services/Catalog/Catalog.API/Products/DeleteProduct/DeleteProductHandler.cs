namespace Catalog.API.Products.DeleteProduct;

// ------------------------------------------------------------
// Command Definition
// ------------------------------------------------------------
// In CQRS architecture, a Command represents an action that
// modifies the application state.
//
// DeleteProductCommand is used to delete an existing product
// from the catalog database.
//
// Parameter:
//
// Id → Unique identifier of the product to be deleted.
//
// The command implements ICommand<DeleteProductResult>,
// meaning the command handler will return a DeleteProductResult.
public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;


// ------------------------------------------------------------
// Command Result
// ------------------------------------------------------------
// This record represents the result returned after executing
// the delete operation.
//
// IsSuccess → Indicates whether the deletion was successful.
public record DeleteProductResult(bool IsSuccess);


// ------------------------------------------------------------
// Command Validator
// ------------------------------------------------------------
// FluentValidation is used to validate the command before
// it reaches the handler.
//
// This validation is automatically triggered through the
// MediatR pipeline using ValidationBehavior.
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        // Ensure the Product Id is provided
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required");
    }
}


// ------------------------------------------------------------
// Command Handler
// ------------------------------------------------------------
// This handler processes the DeleteProductCommand.
//
// Responsibilities:
// 1. Receive the command from MediatR
// 2. Delete the product from the database
// 3. Persist the changes
// 4. Return the operation result
//
// IDocumentSession is part of Marten and provides access
// to PostgreSQL document storage.
internal class DeleteProductCommandHandler
    (IDocumentSession session)

    // Implements ICommandHandler linking the command
    // with its result type.
    : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Mark Product for Deletion
        // --------------------------------------------------------
        // session.Delete<T>(id) marks the document for deletion
        // using its primary key.
        //
        // Marten will remove the corresponding record
        // from the PostgreSQL document table.
        session.Delete<Product>(command.Id);


        // --------------------------------------------------------
        // Step 2: Persist Changes to Database
        // --------------------------------------------------------
        // SaveChangesAsync commits the deletion to the database.
        await session.SaveChangesAsync(cancellationToken);


        // --------------------------------------------------------
        // Step 3: Return Result
        // --------------------------------------------------------
        return new DeleteProductResult(true);
    }
}
