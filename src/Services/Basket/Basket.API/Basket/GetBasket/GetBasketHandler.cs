namespace Basket.API.Basket.GetBasket;

// ------------------------------------------------------------
// Query Definition
// ------------------------------------------------------------
// In CQRS architecture, a Query represents a request to read
// data from the system without modifying any state.
//
// GetBasketQuery is used to retrieve the shopping basket
// associated with a specific user.
//
// Parameter:
//
// UserName → The unique identifier for the user whose
// shopping basket we want to retrieve.
//
// Example request:
// GET /basket/{username}
//
// This query implements IQuery<GetBasketResult>, meaning
// the handler will return a GetBasketResult object.
public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;


// ------------------------------------------------------------
// Query Result
// ------------------------------------------------------------
// This record represents the response returned after
// executing the query.
//
// Cart → ShoppingCart object containing the user's basket
// including all selected items.
public record GetBasketResult(ShoppingCart Cart);


// ------------------------------------------------------------
// Query Handler
// ------------------------------------------------------------
// This handler processes the GetBasketQuery.
//
// Responsibilities:
// 1. Receive the query through MediatR
// 2. Retrieve the basket from the repository
// 3. Return the basket as a query result
//
// IBasketRepository abstracts the data access logic
// for retrieving basket data.
//
// In this project, the repository typically interacts
// with Redis (Distributed Cache) where shopping carts
// are stored.
public class GetBasketQueryHandler(IBasketRepository repository)

    // Implements IQueryHandler linking the query with
    // its response type.
    : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(
        GetBasketQuery query,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // Step 1: Retrieve Basket from Repository
        // --------------------------------------------------------
        // The repository fetches the user's shopping cart
        // from the Redis cache using the provided username.
        var basket = await repository.GetBasket(query.UserName);


        // --------------------------------------------------------
        // Step 2: Return Query Result
        // --------------------------------------------------------
        // Wrap the retrieved shopping cart into
        // GetBasketResult and return it.
        return new GetBasketResult(basket);
    }
}
