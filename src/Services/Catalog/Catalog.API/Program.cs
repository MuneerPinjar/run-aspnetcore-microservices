using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

// ------------------------------------------------------------
// Application Builder Initialization
// ------------------------------------------------------------
// WebApplication.CreateBuilder initializes the ASP.NET Core
// application and prepares configuration, logging, and DI
// (Dependency Injection) container.
var builder = WebApplication.CreateBuilder(args);


// ------------------------------------------------------------
// Register Services (Dependency Injection Container)
// ------------------------------------------------------------

// Get the current assembly reference.
// This is used later to automatically scan and register
// MediatR handlers and FluentValidation validators.
var assembly = typeof(Program).Assembly;


// ------------------------------------------------------------
// MediatR Registration
// ------------------------------------------------------------
// MediatR is used to implement the CQRS pattern.
// It allows sending commands/queries to handlers
// without tightly coupling controllers to business logic.
builder.Services.AddMediatR(config =>
{
    // Automatically register all MediatR handlers
    // (CommandHandlers, QueryHandlers) from this assembly.
    config.RegisterServicesFromAssembly(assembly);

    // Add pipeline behavior for validation.
    // This ensures that FluentValidation runs BEFORE
    // a command/query reaches the handler.
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));

    // Add logging behavior to log requests/responses
    // passing through MediatR pipeline.
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});


// ------------------------------------------------------------
// FluentValidation Registration
// ------------------------------------------------------------
// Automatically register all validators present in the
// current assembly.
builder.Services.AddValidatorsFromAssembly(assembly);


// ------------------------------------------------------------
// Carter Registration
// ------------------------------------------------------------
// Carter is a library used to organize Minimal API endpoints
// into modular components.
//
// Instead of putting all routes in Program.cs,
// endpoints are placed into ICarterModule classes.
builder.Services.AddCarter();


// ------------------------------------------------------------
// Marten Configuration (PostgreSQL Document DB)
// ------------------------------------------------------------
// Marten is used as the data persistence layer.
// It stores .NET objects as documents in PostgreSQL.
builder.Services.AddMarten(opts =>
{
    // Configure PostgreSQL connection string
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);

})
// Use lightweight sessions for better performance.
// Lightweight sessions skip identity tracking and
// are faster for stateless operations.
.UseLightweightSessions();


// ------------------------------------------------------------
// Development Environment Setup
// ------------------------------------------------------------
// If the application is running in development mode,
// automatically initialize the database with seed data.
if (builder.Environment.IsDevelopment())

    builder.Services.InitializeMartenWith<CatalogInitialData>();


// ------------------------------------------------------------
// Global Exception Handling
// ------------------------------------------------------------
// Register a custom exception handler to capture
// unhandled exceptions and return standardized
// error responses.
builder.Services.AddExceptionHandler<CustomExceptionHandler>();


// ------------------------------------------------------------
// Health Checks
// ------------------------------------------------------------
// Health checks help monitoring tools determine
// whether the application and its dependencies
// are running correctly.
builder.Services.AddHealthChecks()

    // Add PostgreSQL health check
    // This ensures the application can connect
    // to the database.
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);


// ------------------------------------------------------------
// Build the Application
// ------------------------------------------------------------
// After registering all services, the application
// instance is created.
var app = builder.Build();


// ------------------------------------------------------------
// Configure HTTP Request Pipeline
// ------------------------------------------------------------


// ------------------------------------------------------------
// Map Carter Endpoints
// ------------------------------------------------------------
// This scans all ICarterModule implementations
// and registers their routes automatically.
app.MapCarter();


// ------------------------------------------------------------
// Global Exception Middleware
// ------------------------------------------------------------
// This enables the previously registered
// CustomExceptionHandler to catch exceptions
// and return proper error responses.
app.UseExceptionHandler(options => { });


// ------------------------------------------------------------
// Health Check Endpoint
// ------------------------------------------------------------
// Exposes a health endpoint used by monitoring
// systems (Kubernetes, Docker, Prometheus, etc).
//
// Example endpoint:
// GET /health
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        // Formats the health check output
        // in a UI-friendly JSON structure.
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });


// ------------------------------------------------------------
// Start the Application
// ------------------------------------------------------------
// Runs the web server and begins listening
// for incoming HTTP requests.
app.Run();
