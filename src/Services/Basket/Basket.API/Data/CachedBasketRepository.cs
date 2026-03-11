using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data;

// ------------------------------------------------------------
// CachedBasketRepository
// ------------------------------------------------------------
// This class implements the IBasketRepository interface and
// acts as a decorator around the main BasketRepository.
//
// It adds Redis caching on top of the repository operations
// to improve performance.
//
// Pattern used here:
// Decorator Pattern
//
// Instead of directly accessing the database every time,
// this repository first checks Redis cache.
//
// Architecture:
//
// Handler
//    │
//    ▼
// CachedBasketRepository
//    │
//    ├── Redis Cache
//    │
//    ▼
// BasketRepository
//    │
//    ▼
// PostgreSQL (via Marten)
//
// Dependencies:
//
// IBasketRepository → underlying repository (database access)
// IDistributedCache → Redis distributed cache
public class CachedBasketRepository
    (IBasketRepository repository, IDistributedCache cache) 
    : IBasketRepository
{

    // --------------------------------------------------------
    // GetBasket
    // --------------------------------------------------------
    // Retrieves the basket for a user.
    //
    // First checks Redis cache.
    // If not found, retrieves it from the database and
    // stores it in the cache.
    public async Task<ShoppingCart> GetBasket(
        string userName,
        CancellationToken cancellationToken = default)
    {
        // ----------------------------------------------------
        // Step 1: Try retrieving basket from Redis cache
        // ----------------------------------------------------
        var cachedBasket = await cache.GetStringAsync(
            userName,
            cancellationToken);

        // If basket exists in cache, deserialize JSON
        // back into a ShoppingCart object.
        if (!string.IsNullOrEmpty(cachedBasket))
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;


        // ----------------------------------------------------
        // Step 2: Fetch basket from underlying repository
        // ----------------------------------------------------
        var basket = await repository.GetBasket(
            userName,
            cancellationToken);


        // ----------------------------------------------------
        // Step 3: Store basket in Redis cache
        // ----------------------------------------------------
        await cache.SetStringAsync(
            userName,
            JsonSerializer.Serialize(basket),
            cancellationToken);


        return basket;
    }


    // --------------------------------------------------------
    // StoreBasket
    // --------------------------------------------------------
    // Stores or updates the basket in the database and cache.
    //
    // Steps:
    // 1. Save basket in database
    // 2. Update Redis cache
    public async Task<ShoppingCart> StoreBasket(
        ShoppingCart basket,
        CancellationToken cancellationToken = default)
    {
        // Save basket in database
        await repository.StoreBasket(
            basket,
            cancellationToken);


        // Update Redis cache with latest basket
        await cache.SetStringAsync(
            basket.UserName,
            JsonSerializer.Serialize(basket),
            cancellationToken);


        return basket;
    }


    // --------------------------------------------------------
    // DeleteBasket
    // --------------------------------------------------------
    // Deletes the basket from both the database and cache.
    //
    // Steps:
    // 1. Remove basket from database
    // 2. Remove basket from Redis cache
    public async Task<bool> DeleteBasket(
        string userName,
        CancellationToken cancellationToken = default)
    {
        // Delete basket from database
        await repository.DeleteBasket(
            userName,
            cancellationToken);


        // Remove cached basket from Redis
        await cache.RemoveAsync(
            userName,
            cancellationToken);


        return true;
    }
}
