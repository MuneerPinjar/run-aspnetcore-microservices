using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Add Services to the Dependency Injection Container
// ------------------------------------------------------------
// ASP.NET Core uses Dependency Injection (DI) to manage
// application services such as repositories, databases,
// messaging, logging, etc.


// ------------------------------------------------------------
// Application Services
// ------------------------------------------------------------

// Get the current assembly so MediatR can scan it
// and register handlers automatically.
var assembly = typeof(Program).Assembly;

// Register Carter modules for Minimal API endpoints
builder.Services.AddCarter();

// Register MediatR for CQRS pattern
builder.Services.AddMediatR(config =>
{
    // Register all command/query handlers in the assembly
    config.RegisterServicesFromAssembly(assembly);

    // Add validation pipeline behavior
    // This runs FluentValidation before handlers execute
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));

    // Add logging pipeline behavior
    // This logs request start/end and performance metrics
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});


// ------------------------------------------------------------
// Data Services
// ------------------------------------------------------------
// Configure Marten (PostgreSQL document database)

builder.Services.AddMarten(opts =>
{
    // Database connection string from configuration
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);

    // Configure ShoppingCart document identity
    // UserName will act as the primary key
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
})

// Lightweight sessions improve performance because
// they do not track entity changes like identity sessions.
.UseLightweightSessions();


// ------------------------------------------------------------
// Repository Registration
// ------------------------------------------------------------

// Register BasketRepository as the default implementation
// for IBasketRepository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// Decorator pattern:
// CachedBasketRepository wraps BasketRepository
// to add Redis caching functionality
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();


// ------------------------------------------------------------
// Redis Distributed Cache
// ------------------------------------------------------------
// Redis is used for storing shopping cart data.

builder.Services.AddStackExchangeRedisCache(options =>
{
    // Redis connection string from configuration
    options.Configuration = builder.Configuration.GetConnectionString("Redis");

    // Optional instance name for key prefix
    // options.InstanceName = "Basket";
});


// ------------------------------------------------------------
// gRPC Client Configuration
// ------------------------------------------------------------
// Register a gRPC client for communicating with the
// Discount microservice.

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    // Address of the Discount gRPC service
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})

// Custom HTTP handler configuration
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        // Accept any SSL certificate
        // Used mainly in development environments
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});


// ------------------------------------------------------------
// Asynchronous Messaging Services
// ------------------------------------------------------------
// Configure MassTransit with RabbitMQ for
// event-driven communication between microservices.

builder.Services.AddMessageBroker(builder.Configuration);


// ------------------------------------------------------------
// Cross-Cutting Services
// ------------------------------------------------------------

// Global exception handler for consistent error responses
builder.Services.AddExceptionHandler<CustomExceptionHandler>();


// ------------------------------------------------------------
// Health Checks
// ------------------------------------------------------------
// Health checks monitor the status of critical dependencies.

builder.Services.AddHealthChecks()

    // PostgreSQL health check
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)

    // Redis health check
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);


// ------------------------------------------------------------
// Build Application
// ------------------------------------------------------------
var app = builder.Build();


// ------------------------------------------------------------
// Configure HTTP Request Pipeline
// ------------------------------------------------------------

// Register Carter endpoints
app.MapCarter();


// Global exception handling middleware
app.UseExceptionHandler(options => { });


// Health check endpoint
// Example: http://localhost:6001/health
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        // Format health response for UI dashboards
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });


// ------------------------------------------------------------
// Run Application
// ------------------------------------------------------------
app.Run();
