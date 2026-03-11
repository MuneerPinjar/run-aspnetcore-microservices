using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

// ------------------------------------------------------------
// LoggingBehavior
// ------------------------------------------------------------
// This class implements MediatR's IPipelineBehavior.
//
// Pipeline behaviors allow adding cross-cutting concerns
// (logging, validation, caching, performance monitoring)
// without modifying handlers.
//
// LoggingBehavior logs:
// 1. When a request starts
// 2. When a request finishes
// 3. How long the request took
// 4. A warning if the request takes too long
//
// Execution order:
//
// API Endpoint
//     │
//     ▼
// LoggingBehavior (Start Log)
//     │
//     ▼
// ValidationBehavior
//     │
//     ▼
// Handler
//     │
//     ▼
// LoggingBehavior (End Log)
public class LoggingBehavior<TRequest, TResponse>

    // Constructor injection for logger
    // ILogger is provided by ASP.NET Core logging infrastructure.
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)

    // This class participates in MediatR's request pipeline.
    : IPipelineBehavior<TRequest, TResponse>

    // ------------------------------------------------------------
    // Generic Constraints
    // ------------------------------------------------------------
    // TRequest must implement IRequest<TResponse>.
    // This ensures the behavior runs for MediatR requests.
    where TRequest : notnull, IRequest<TResponse>

    // TResponse must not be null.
    where TResponse : notnull
{
    // ------------------------------------------------------------
    // Handle Method
    // ------------------------------------------------------------
    // This method intercepts every MediatR request.
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Log Request Start
        // --------------------------------------------------------
        // Logs the request type, response type, and request data.
        logger.LogInformation(
            "[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            typeof(TRequest).Name,
            typeof(TResponse).Name,
            request
        );


        // --------------------------------------------------------
        // Start Performance Timer
        // --------------------------------------------------------
        // Stopwatch measures how long the request takes.
        var timer = new Stopwatch();
        timer.Start();


        // --------------------------------------------------------
        // Execute Next Pipeline Step
        // --------------------------------------------------------
        // This calls the next behavior or the actual handler.
        var response = await next();


        // --------------------------------------------------------
        // Stop Timer
        // --------------------------------------------------------
        timer.Stop();
        var timeTaken = timer.Elapsed;


        // --------------------------------------------------------
        // Performance Warning
        // --------------------------------------------------------
        // If the request takes more than 3 seconds,
        // log a warning for performance investigation.
        if (timeTaken.Seconds > 3)
            logger.LogWarning(
                "[PERFORMANCE] The request {Request} took {TimeTaken} seconds.",
                typeof(TRequest).Name,
                timeTaken.Seconds
            );


        // --------------------------------------------------------
        // Log Request Completion
        // --------------------------------------------------------
        logger.LogInformation(
            "[END] Handled {Request} with {Response}",
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );


        // --------------------------------------------------------
        // Return Handler Response
        // --------------------------------------------------------
        return response;
    }
}
