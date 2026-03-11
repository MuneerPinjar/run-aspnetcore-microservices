using BuildingBlocks.Exceptions;

namespace Catalog.API.Exceptions;

// ------------------------------------------------------------
// ProductNotFoundException
// ------------------------------------------------------------
// This is a custom exception used when a Product cannot
// be found in the database.
//
// It inherits from NotFoundException which is defined in
// the BuildingBlocks.Exceptions namespace.
//
// The purpose of creating specific exceptions like this is:
// 1. Provide clear domain-specific error messages
// 2. Enable centralized exception handling
// 3. Return meaningful HTTP responses to clients
//
// Example scenario:
// A client requests a product using an Id that does not exist
// in the database.
//
// Example API request:
// GET /products/{id}
//
// If the product does not exist, this exception will be thrown
// from the QueryHandler.
public class ProductNotFoundException : NotFoundException
{
    // --------------------------------------------------------
    // Constructor
    // --------------------------------------------------------
    // Guid Id -> The unique identifier of the product
    // that was requested but not found.
    //
    // This constructor passes two parameters to the base
    // NotFoundException:
    //
    // "Product" -> Name of the entity
    // Id        -> Identifier of the missing entity
    //
    // The base exception typically formats a message like:
    //
    // "Product with Id 'xxxx-xxxx-xxxx' was not found."
    public ProductNotFoundException(Guid Id) : base("Product", Id)
    {
    }
}
