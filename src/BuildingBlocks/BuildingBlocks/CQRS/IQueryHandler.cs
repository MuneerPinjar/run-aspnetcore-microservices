using MediatR;

namespace BuildingBlocks.CQRS;

// ------------------------------------------------------------
// IQueryHandler<TQuery, TResponse>
// ------------------------------------------------------------
// This interface represents a handler responsible for processing
// CQRS Queries.
//
// In CQRS (Command Query Responsibility Segregation):
//
// Commands → Modify system state (INSERT, UPDATE, DELETE)
// Queries  → Retrieve data (READ operations)
//
// QueryHandlers execute read-only operations and return data
// from the database, cache, or external services.
//
// This interface extends MediatR's IRequestHandler which allows
// MediatR to automatically route queries to the correct handler.
//
// IRequestHandler<TRequest, TResponse> requires implementing:
//
// Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
//
// Meaning any class implementing IQueryHandler must implement
// the Handle() method that processes the query and returns the
// expected response.

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>

    // ------------------------------------------------------------
    // Generic Constraints
    // ------------------------------------------------------------

    // Ensures that the request being handled is a valid Query
    // implementing the IQuery<TResponse> interface.
    //
    // This enforces CQRS architecture rules by ensuring only
    // query types are handled here.
    where TQuery : IQuery<TResponse>

    // Ensures that the query response cannot be null.
    // This improves API safety and prevents null reference issues.
    where TResponse : notnull
{
}
