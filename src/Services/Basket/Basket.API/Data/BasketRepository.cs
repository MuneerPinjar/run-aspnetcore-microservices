namespace Basket.API.Data;

// ------------------------------------------------------------
// BasketRepository
// ------------------------------------------------------------
// This class is the concrete implementation of IBasketRepository.
//
// It handles all basket-related data operations using Marten,
// which is a PostgreSQL document database library for .NET.
//
// Marten stores .NET objects as JSON documents inside PostgreSQL.
//
// Dependency:
// IDocumentSession → Marten session used to interact with
// the document database.
public class BasketRepository(IDocumentSession session)
    : IBasketRepository
{

    // --------------------------------------------------------
    // GetBasket
    // --------------------------------------------------------
    // Retrieves a shopping basket for a specific user.
    //
    // Parameters:
    // userName → Identifier of the user whose basket we want
    // cancellationToken → Allows cancellation of async operation
    //
    // Marten uses LoadAsync<T>() to fetch a document by its
    // identity (primary key).
    public async Task<ShoppingCart> GetBasket(
        string userName,
        CancellationToken cancellationToken = default)
    {
        // Attempt to load the ShoppingCart document
        // using the username as the document ID.
        var basket = await session.LoadAsync<ShoppingCart>(
            userName,
            cancellationToken);

        // If the basket does not exist, throw a custom exception.
        // This will later be handled by CustomExceptionHandler
        // and returned as HTTP 404.
        return basket is null
            ? throw new BasketNotFoundException(userName)
            : basket;
    }


    // --------------------------------------------------------
    // StoreBasket
    // --------------------------------------------------------
    // Saves or updates a shopping basket in the database.
    //
    // Marten's Store() method performs an UPSERT operation:
    //
    // Insert → if document does not exist
    // Update → if document already exists
    public async Task<ShoppingCart> StoreBasket(
        ShoppingCart basket,
        CancellationToken cancellationToken = default)
    {
        // Store the ShoppingCart document
        session.Store(basket);

        // Persist changes to PostgreSQL
        await session.SaveChangesAsync(cancellationToken);

        // Return the stored basket
        return basket;
    }


    // --------------------------------------------------------
    // DeleteBasket
    // --------------------------------------------------------
    // Deletes a shopping basket for a given user.
    //
    // Parameters:
    // userName → User whose basket should be deleted
    //
    // Marten allows deleting a document by identity.
    public async Task<bool> DeleteBasket(
        string userName,
        CancellationToken cancellationToken = default)
    {
        // Mark the ShoppingCart document for deletion
        session.Delete<ShoppingCart>(userName);

        // Persist deletion to PostgreSQL
        await session.SaveChangesAsync(cancellationToken);

        // Return success indicator
        return true;
    }
}
