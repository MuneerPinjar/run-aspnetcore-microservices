namespace BuildingBlocks.Exceptions;

// ------------------------------------------------------------
// InternalServerException
// ------------------------------------------------------------
// This is a custom exception used to represent unexpected
// server-side errors in the application.
//
// It extends the base .NET Exception class and provides
// additional context for internal failures.
//
// Typical scenarios:
// • Database connection failures
// • External service errors
// • Unexpected runtime errors
//
// In your architecture, this exception will be caught by the
// CustomExceptionHandler and mapped to:
//
// HTTP 500 Internal Server Error
public class InternalServerException : Exception
{
    // ------------------------------------------------------------
    // Constructor 1
    // ------------------------------------------------------------
    // Accepts a simple error message.
    //
    // Example usage:
    // throw new InternalServerException("Unexpected server error");
    public InternalServerException(string message)
        : base(message)
    {
    }


    // ------------------------------------------------------------
    // Constructor 2
    // ------------------------------------------------------------
    // Allows providing additional diagnostic information.
    //
    // Parameters:
    //
    // message → Main error message
    // details → Additional debugging information
    //
    // Example:
    //
    // throw new InternalServerException(
    //     "Database operation failed",
    //     "Connection timeout while accessing PostgreSQL"
    // );
    public InternalServerException(string message, string details)
        : base(message)
    {
        // Store additional error details
        Details = details;
    }


    // ------------------------------------------------------------
    // Details Property
    // ------------------------------------------------------------
    // Optional additional information describing the error.
    //
    // This can be useful for:
    // • Logging
    // • Debugging
    // • Diagnostic tracing
    //
    // The '?' indicates this property can be null.
    public string? Details { get; }
}
