namespace Catalog.API.Products.GetProductById;

// ------------------------------------------------------------
// Query Definition
// ------------------------------------------------------------
// In CQRS, a Query represents a request to retrieve data
// without modifying application state.
//
// GetProductByIdQuery is used to fetch a single product
// from the catalog database using its unique identifier.
//
// Guid Id → unique identifier of the product.
//
// This query implements IQuery<GetProductByIdResult>,
// meaning its handler will return a GetProductByIdResult object.
public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;


// ------------------------------------------------------------
// Query Result
// ------------------------------------------------------------
// This record represents the response returned after the
// query execution.
//
// It contains the Product entity that was retrieved
// from the database.
public record GetProductByIdResult(Product Product);


// ------------------------------------------------------------
// Query Handler
// ------------------------------------------------------------
// This handler processes the GetProductByIdQuery.
//
// Responsibilities:
// 1. Receive the query through MediatR
// 2. Fetch the product from the database using Marten
// 3. Validate that the product exists
// 4. Return the result
//
// IDocumentSession is part of Marten and provides
// access to PostgreSQL document storage.
internal class GetProductByIdQueryHandler
    (IDocumentSession session)

    // Implements IQueryHandler which links the query
    // with its response type.
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Retrieve Product from Database
        // --------------------------------------------------------
        // LoadAsync is a Marten method used to retrieve
        // a document by its primary key (Id).
        //
        // If the product exists, it will be returned.
        // Otherwise, it returns null.
        var product = await session.LoadAsync<Product>(
            query.Id,
            cancellationToken
        );


        // --------------------------------------------------------
        // Step 2: Handle Product Not Found
        // --------------------------------------------------------
        // If the product does not exist, throw a custom exception.
        //
        // This exception will be captured by the global
        // CustomExceptionHandler middleware and returned
        // as a proper HTTP error response.
        if (product is null)
        {
            throw new ProductNotFoundException(query.Id);
        }


        // --------------------------------------------------------
        // Step 3: Return Query Result
        // --------------------------------------------------------
        // Wrap the product inside a result object
        // and return it to the API layer.
        return new GetProductByIdResult(product);
    }
}
