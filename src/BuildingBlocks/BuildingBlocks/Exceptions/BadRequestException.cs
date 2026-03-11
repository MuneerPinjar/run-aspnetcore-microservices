namespace BuildingBlocks.Exceptions;

// ------------------------------------------------------------
// BadRequestException
// ------------------------------------------------------------
// This is a custom exception used to represent invalid client
// requests (HTTP 400 Bad Request).
//
// It extends the base .NET Exception class and allows the
// application to throw meaningful errors when the client sends
// incorrect or invalid input.
//
// Typical scenarios:
// • Invalid request parameters
// • Business rule violations
// • Invalid command data
// • Domain validation errors
//
// In your architecture this exception will be captured by the
// CustomExceptionHandler and converted into:
//
// HTTP 400 Bad Request response.
public class BadRequestException : Exception
{
    // ------------------------------------------------------------
    // Constructor 1
    // ------------------------------------------------------------
    // Accepts a simple error message describing the problem.
    //
    // Example usage:
    // throw new BadRequestException("Invalid product price");
    public BadRequestException(string message)
        : base(message)
    {
    }


    // ------------------------------------------------------------
    // Constructor 2
    // ------------------------------------------------------------
    // Allows passing additional details about the error.
    //
    // Parameters:
    //
    // message → Main error message
    // details → Additional debugging information
    //
    // Example usage:
    //
    // throw new BadRequestException(
    //     "Invalid request",
    //     "Price cannot be negative"
    // );
    public BadRequestException(string message, string details)
        : base(message)
    {
        // Store extra information about the error
        Details = details;
    }


    // ------------------------------------------------------------
    // Details Property
    // ------------------------------------------------------------
    // Optional property containing extra information about
    // the error.
    //
    // This is useful for:
    // • Debugging
    // • Logging
    // • Diagnostics
    //
    // The '?' indicates that the property can be null.
    public string? Details { get; }
}
