namespace Catalog.API.Products.UpdateProduct;

// ------------------------------------------------------------
// Command Definition
// ------------------------------------------------------------
// In CQRS, a Command represents an intention to modify
// application state (Create, Update, Delete operations).
//
// UpdateProductCommand is used to update an existing product
// in the catalog database.
//
// Parameters:
//
// Id          → Unique identifier of the product
// Name        → Updated product name
// Category    → Updated list of product categories
// Description → Updated description
// ImageFile   → Updated image filename/path
// Price       → Updated product price
//
// The command implements ICommand<UpdateProductResult>,
// meaning its handler will return an UpdateProductResult.
public record UpdateProductCommand(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<UpdateProductResult>;


// ------------------------------------------------------------
// Command Result
// ------------------------------------------------------------
// This record represents the result returned after
// executing the update command.
//
// IsSuccess → indicates whether the update operation
// completed successfully.
public record UpdateProductResult(bool IsSuccess);


// ------------------------------------------------------------
// Command Validator
// ------------------------------------------------------------
// FluentValidation is used to validate incoming commands
// before they reach the command handler.
//
// This validation runs through the MediatR pipeline
// (ValidationBehavior).
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        // Ensure Product Id is provided
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Product ID is required");

        // Validate Product Name
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Name is required")

            // Restrict name length to avoid invalid values
            .Length(2, 150)
            .WithMessage("Name must be between 2 and 150 characters");

        // Ensure Price is valid
        RuleFor(command => command.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}


// ------------------------------------------------------------
// Command Handler
// ------------------------------------------------------------
// This handler processes the UpdateProductCommand.
//
// Responsibilities:
// 1. Retrieve the existing product from the database
// 2. Validate that the product exists
// 3. Update the product fields
// 4. Persist changes to the database
// 5. Return the command result
//
// IDocumentSession is part of Marten and provides
// access to PostgreSQL document storage.
internal class UpdateProductCommandHandler
    (IDocumentSession session)

    // Implements ICommandHandler linking the command
    // to its corresponding response type.
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Load Existing Product from Database
        // --------------------------------------------------------
        // LoadAsync retrieves a document using its primary key.
        var product = await session.LoadAsync<Product>(
            command.Id,
            cancellationToken
        );

        // --------------------------------------------------------
        // Step 2: Handle Product Not Found
        // --------------------------------------------------------
        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        }

        // --------------------------------------------------------
        // Step 3: Update Product Properties
        // --------------------------------------------------------
        // Update the product fields using values
        // provided in the command.
        product.Name = command.Name;
        product.Category = command.Category;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;

        // --------------------------------------------------------
        // Step 4: Mark Document as Updated
        // --------------------------------------------------------
        // session.Update() tells Marten that the document
        // has been modified and should be persisted.
        session.Update(product);

        // --------------------------------------------------------
        // Step 5: Save Changes to Database
        // --------------------------------------------------------
        await session.SaveChangesAsync(cancellationToken);

        // --------------------------------------------------------
        // Step 6: Return Command Result
        // --------------------------------------------------------
        return new UpdateProductResult(true);
    }
}
