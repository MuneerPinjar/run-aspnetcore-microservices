namespace Catalog.API.Products.GetProductByCategory;

// ------------------------------------------------------------
// Query Definition
// ------------------------------------------------------------
// In CQRS architecture, a Query represents a request to
// retrieve data without modifying the application state.
//
// GetProductByCategoryQuery is used to fetch products that
// belong to a specific category.
//
// Example request:
// GET /products/category/Electronics
//
// Parameter:
// Category → Name of the category used to filter products.
//
// The query implements IQuery<GetProductByCategoryResult>
// which means the handler will return a GetProductByCategoryResult.
public record GetProductByCategoryQuery(string Category)
    : IQuery<GetProductByCategoryResult>;


// ------------------------------------------------------------
// Query Result
// ------------------------------------------------------------
// This record represents the response returned after
// executing the query.
//
// It contains a collection of Product entities that match
// the specified category.
public record GetProductByCategoryResult(IEnumerable<Product> Products);


// ------------------------------------------------------------
// Query Handler
// ------------------------------------------------------------
// This handler processes the GetProductByCategoryQuery.
//
// Responsibilities:
// 1. Receive the query from MediatR
// 2. Filter products based on the category
// 3. Retrieve matching products from the database
// 4. Return the result
//
// IDocumentSession is provided by Marten and allows
// interaction with the PostgreSQL document database.
internal class GetProductByCategoryQueryHandler
    (IDocumentSession session)

    // Implements IQueryHandler which connects the query
    // with its corresponding result type.
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(
        GetProductByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Query the Product collection from Marten
        // --------------------------------------------------------
        // session.Query<Product>() returns an IQueryable<Product>
        // allowing LINQ operations against the database.
        var products = await session.Query<Product>()

            // ----------------------------------------------------
            // Filter products by category
            // ----------------------------------------------------
            // Each Product contains a list of categories:
            //
            // Example:
            // Category = ["Electronics", "Mobile"]
            //
            // This filter checks whether the requested category
            // exists inside that list.
            .Where(p => p.Category.Contains(query.Category))

            // ----------------------------------------------------
            // Execute query and convert results into a list
            // ----------------------------------------------------
            .ToListAsync(cancellationToken);

        // --------------------------------------------------------
        // Return Query Result
        // --------------------------------------------------------
        return new GetProductByCategoryResult(products);
    }
}
