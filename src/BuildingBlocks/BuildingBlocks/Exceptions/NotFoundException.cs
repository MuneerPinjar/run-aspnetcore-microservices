namespace BuildingBlocks.Exceptions;

// ------------------------------------------------------------
// NotFoundException
// ------------------------------------------------------------
// This is a custom base exception used when a requested
// resource cannot be found in the system.
//
// It extends the standard .NET Exception class.
//
// Purpose:
// • Represent "resource not found" scenarios
// • Provide meaningful error messages
// • Allow centralized exception handling
//
// In your architecture this exception is used for cases like:
// - Product not found
// - Order not found
// - Basket not found
//
// The CustomExceptionHandler maps this exception to:
// HTTP 404 Not Found
public class NotFoundException : Exception
{
    // ------------------------------------------------------------
    // Constructor 1
    // ------------------------------------------------------------
    // Allows passing a custom error message.
    //
    // Example:
    // throw new NotFoundException("Product was not found");
    public NotFoundException(string message)
        : base(message)
    {
    }


    // ------------------------------------------------------------
    // Constructor 2
    // ------------------------------------------------------------
    // A more structured constructor used when an entity
    // is missing based on its identifier.
    //
    // Parameters:
    //
    // name → Entity name (e.g., Product, Order)
    // key  → Identifier value (e.g., ProductId)
    //
    // Example usage:
    //
    // throw new NotFoundException("Product", productId);
    //
    // Generated message:
    // Entity "Product" (123) was not found.
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
