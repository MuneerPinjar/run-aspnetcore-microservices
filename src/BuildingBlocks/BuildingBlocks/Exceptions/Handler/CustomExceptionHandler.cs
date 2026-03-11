using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler;

// ------------------------------------------------------------
// CustomExceptionHandler
// ------------------------------------------------------------
// This class implements ASP.NET Core's IExceptionHandler.
//
// It provides a centralized way to handle exceptions across
// the entire application.
//
// Instead of handling exceptions in each controller or endpoint,
// this handler captures all unhandled exceptions and converts
// them into standardized HTTP responses.
//
// Benefits:
// • Centralized error handling
// • Consistent API error responses
// • Proper HTTP status codes
// • Logging for debugging and monitoring
public class CustomExceptionHandler
    (ILogger<CustomExceptionHandler> logger)

    // ASP.NET Core interface used for global exception handling
    : IExceptionHandler
{
    // ------------------------------------------------------------
    // TryHandleAsync Method
    // ------------------------------------------------------------
    // This method is called automatically whenever an unhandled
    // exception occurs in the application.
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Log Exception
        // --------------------------------------------------------
        // Logs the error message and the time it occurred.
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message,
            DateTime.UtcNow
        );


        // --------------------------------------------------------
        // Determine Error Details Based on Exception Type
        // --------------------------------------------------------
        // Pattern matching is used to map specific exceptions
        // to appropriate HTTP status codes and messages.
        (string Detail, string Title, int StatusCode) details = exception switch
        {
            // Internal server errors
            InternalServerException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),

            // Validation errors (FluentValidation)
            ValidationException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),

            // Custom bad request exceptions
            BadRequestException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),

            // Resource not found errors
            NotFoundException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status404NotFound
            ),

            // Default case for unknown exceptions
            _ =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };


        // --------------------------------------------------------
        // Create ProblemDetails Object
        // --------------------------------------------------------
        // ProblemDetails is a standard structure defined by
        // RFC 7807 for returning API errors.
        //
        // This ensures consistent error responses across APIs.
        var problemDetails = new ProblemDetails
        {
            Title = details.Title,          // Exception type
            Detail = details.Detail,        // Error message
            Status = details.StatusCode,    // HTTP status code
            Instance = context.Request.Path // Request path
        };


        // --------------------------------------------------------
        // Add Trace Identifier
        // --------------------------------------------------------
        // Useful for debugging and correlating logs.
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);


        // --------------------------------------------------------
        // Add Validation Errors (if applicable)
        // --------------------------------------------------------
        // FluentValidation provides detailed validation errors.
        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add(
                "ValidationErrors",
                validationException.Errors
            );
        }


        // --------------------------------------------------------
        // Return Error Response as JSON
        // --------------------------------------------------------
        await context.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken: cancellationToken
        );

        // Returning true indicates the exception was handled.
        return true;
    }
}
