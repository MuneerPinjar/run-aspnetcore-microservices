namespace Catalog.API.Products.CreateProduct;

// ------------------------------------------------------------
// Command Record
// ------------------------------------------------------------
// In CQRS (Command Query Responsibility Segregation), a Command
// represents an intention to perform an action that changes state.
//
// CreateProductCommand contains all the data required to create
// a new Product in the system.
//
// record:
// - Immutable by default
// - Value-based equality
// - Ideal for commands and DTOs
//
// This command implements ICommand<CreateProductResult>
// meaning it expects a response of type CreateProductResult
// after execution.
public record CreateProductCommand(
    string Name,              // Name of the product
    List<string> Category,    // List of categories the product belongs to
    string Description,       // Product description
    string ImageFile,         // Image filename or URL
    decimal Price             // Product price
) : ICommand<CreateProductResult>;


// ------------------------------------------------------------
// Command Result
// ------------------------------------------------------------
// After the command is executed successfully, the handler
// returns CreateProductResult.
//
// Only the newly created Product Id is returned here.
// This helps the client reference the created resource.
public record CreateProductResult(Guid Id);


// ------------------------------------------------------------
// Command Validator
// ------------------------------------------------------------
// FluentValidation is used here to validate the command
// before it reaches the command handler.
//
// This ensures that invalid data does not reach the
// business logic or database layer.
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        // Ensure Name is not empty
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        // Ensure Category list is not empty
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required");

        // Ensure ImageFile field is provided
        RuleFor(x => x.ImageFile)
            .NotEmpty()
            .WithMessage("ImageFile is required");

        // Ensure Price is greater than 0
        // Prevents free or negative priced products
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}


// ------------------------------------------------------------
// Command Handler
// ------------------------------------------------------------
// Handles the execution of CreateProductCommand.
//
// This class is responsible for:
// 1. Receiving the command
// 2. Creating the domain entity
// 3. Persisting the entity to the database
// 4. Returning the result
//
// The handler uses Marten's IDocumentSession for persistence.
internal class CreateProductCommandHandler
    (IDocumentSession session) // Dependency Injection of Marten session
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // Step 1: Create Product entity from incoming command
        // ------------------------------------------------------------
        // The command is a DTO containing user input.
        // Here we convert it into a domain entity (Product).
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        // ------------------------------------------------------------
        // Step 2: Store the entity in Marten session
        // ------------------------------------------------------------
        // session.Store() tracks the entity for persistence.
        // It does NOT immediately save to the database.
        session.Store(product);

        // ------------------------------------------------------------
        // Step 3: Persist changes to PostgreSQL
        // ------------------------------------------------------------
        // SaveChangesAsync commits all tracked changes
        // within the current Marten session to the database.
        await session.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------
        // Step 4: Return the result
        // ------------------------------------------------------------
        // After saving, Marten generates the Id for the product.
        // We return this Id so the client knows the created resource.
        return new CreateProductResult(product.Id);
    }
}
