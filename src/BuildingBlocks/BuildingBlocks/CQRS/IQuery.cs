using MediatR;

namespace BuildingBlocks.CQRS;

// ------------------------------------------------------------
// IQuery<TResponse>
// ------------------------------------------------------------
// This interface represents a CQRS Query.
//
// In CQRS (Command Query Responsibility Segregation),
// Queries are responsible for retrieving data WITHOUT
// modifying application state.
//
// Key characteristics of Queries:
// 1. They only READ data
// 2. They do NOT change database state
// 3. They always return a result
//
// This interface acts as a base contract for all query
// objects in the application.
//
// Example:
// public record GetProductByIdQuery(Guid Id)
//     : IQuery<GetProductByIdResult>;
//
// The query will be sent through MediatR and handled by
// a corresponding QueryHandler.
//
// Flow:
// API Endpoint → Mediator.Send(Query) → QueryHandler → Result
public interface IQuery<out TResponse> : IRequest<TResponse>

    // ------------------------------------------------------------
    // Generic Constraint
    // ------------------------------------------------------------
    // where TResponse : notnull
    //
    // This ensures that the query MUST return a non-null response.
    // It prevents returning null values which could lead to
    // runtime errors or inconsistent API responses.
    //
    // Example valid responses:
    // - DTO objects
    // - Result objects
    // - Collections
    //
    // Example:
    // IQuery<ProductDto>
    // IQuery<List<ProductDto>>
    //
    // Returning null would violate this constraint.
    where TResponse : notnull
{
}
