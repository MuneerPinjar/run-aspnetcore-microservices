namespace Basket.API.Data;

// ------------------------------------------------------------
// IBasketRepository Interface
// ------------------------------------------------------------
// This interface defines the contract for basket-related
// data operations.
//
// It follows the Repository Pattern, which abstracts
// the data access layer from the application logic.
//
// Instead of handlers directly accessing Redis or the
// database, they communicate through this interface.
//
// Benefits of using a repository:
// • Decouples business logic from data storage
// • Improves testability (can be mocked in unit tests)
// • Allows switching storage implementations easily
// • Centralizes data access logic
public interface IBasketRepository
{
    // --------------------------------------------------------
    // GetBasket
    // --------------------------------------------------------
    // Retrieves the shopping cart associated with a user.
    //
    // Parameters:
    // userName → Unique identifier of the user
    // cancellationToken → Allows cancelling async operations
    //
    // Returns:
    // ShoppingCart object containing basket items.
    //
    // Example usage:
    // var basket = await repository.GetBasket("muneer");
    Task<ShoppingCart> GetBasket(
        string userName,
        CancellationToken cancellationToken = default);


    // --------------------------------------------------------
    // StoreBasket
    // --------------------------------------------------------
    // Saves or updates the user's shopping basket.
    //
    // Parameters:
    // basket → ShoppingCart object containing items
    // cancellationToken → Optional async cancellation
    //
    // Returns:
    // Updated ShoppingCart after storage.
    //
    // Example usage:
    // await repository.StoreBasket(cart);
    Task<ShoppingCart> StoreBasket(
        ShoppingCart basket,
        CancellationToken cancellationToken = default);


    // --------------------------------------------------------
    // DeleteBasket
    // --------------------------------------------------------
    // Deletes the basket associated with a user.
    //
    // Parameters:
    // userName → User whose basket should be removed
    // cancellationToken → Optional async cancellation
    //
    // Returns:
    // true  → Basket successfully deleted
    // false → Basket not found or deletion failed
    //
    // Example usage:
    // await repository.DeleteBasket("muneer");
    Task<bool> DeleteBasket(
        string userName,
        CancellationToken cancellationToken = default);
}
