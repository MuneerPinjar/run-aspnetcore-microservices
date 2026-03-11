
namespace Catalog.API.Products.GetProducts;

// ------------------------------------------------------------
// Query Definition
// ------------------------------------------------------------
// In CQRS, a Query represents a request to retrieve data
// without modifying application state.
//
// GetProductsQuery is used to fetch a list of products
// from the catalog database.
//
// Parameters:
// PageNumber → which page of results to retrieve
// PageSize   → number of records per page
//
// Default values are provided so that if the client does not
// pass these values, the system will automatically use:
// PageNumber = 1
// PageSize = 10
//
// This query implements IQuery<GetProductsResult>, meaning
// the query handler will return a GetProductsResult object.
public record GetProductsQuery(
    int? PageNumber = 1,
    int? PageSize = 10
) : IQuery<GetProductsResult>;


// ------------------------------------------------------------
// Query Result
// ------------------------------------------------------------
// This record represents the response returned after
// the query is executed.
//
// It contains a collection of Product entities retrieved
// from the database.
//
// IEnumerable<Product> is used because the result
// represents multiple product records.
public record GetProductsResult(IEnumerable<Product> Products);


// ------------------------------------------------------------
// Query Handler
// ------------------------------------------------------------
// This handler processes the GetProductsQuery.
//
// Responsibilities:
// 1. Receive the query from MediatR
// 2. Fetch products from the database
// 3. Apply pagination
// 4. Return the result
//
// IDocumentSession is part of Marten and is used to
// interact with PostgreSQL in a document-based manner.
internal class GetProductsQueryHandler
    (IDocumentSession session)

    // The handler implements IQueryHandler which connects
    // the query with its response type.
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Query the Product collection from Marten
        // --------------------------------------------------------
        // session.Query<Product>() returns an IQueryable<Product>
        // allowing LINQ queries against the PostgreSQL database.
        var products = await session.Query<Product>()

            // ----------------------------------------------------
            // Apply Pagination
            // ----------------------------------------------------
            // ToPagedListAsync is a Marten extension method
            // that retrieves only the required subset of data.
            //
            // If PageNumber or PageSize is null,
            // fallback default values are used.
            .ToPagedListAsync(
                query.PageNumber ?? 1,
                query.PageSize ?? 10,
                cancellationToken);

        // --------------------------------------------------------
        // Return Query Result
        // --------------------------------------------------------
        // The products retrieved from the database are wrapped
        // inside a GetProductsResult object and returned.
        return new GetProductsResult(products);
    }
}
