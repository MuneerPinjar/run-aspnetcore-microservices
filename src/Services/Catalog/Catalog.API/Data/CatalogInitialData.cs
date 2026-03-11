using Marten.Schema;

namespace Catalog.API.Data;

// ------------------------------------------------------------
// CatalogInitialData
// ------------------------------------------------------------
// This class is responsible for seeding (pre-populating)
// the database with initial product data when the application
// starts.
//
// It implements Marten's IInitialData interface.
//
// Marten automatically executes this class during application
// startup when configured in Program.cs:
//
// builder.Services.InitializeMartenWith<CatalogInitialData>();
//
// This is typically used for:
// • Development environments
// • Demo/testing data
// • Initial system setup
public class CatalogInitialData : IInitialData
{
    // ------------------------------------------------------------
    // Populate Method
    // ------------------------------------------------------------
    // This method is automatically called by Marten
    // when the application starts.
    //
    // Parameters:
    // store → Marten document store used to create sessions
    // cancellation → cancellation token for async operations
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        // --------------------------------------------------------
        // Create Lightweight Session
        // --------------------------------------------------------
        // LightweightSession is optimized for stateless operations.
        // It is faster because it does not track entities
        // like IdentitySession.
        using var session = store.LightweightSession();


        // --------------------------------------------------------
        // Check if Data Already Exists
        // --------------------------------------------------------
        // If any product already exists in the database,
        // we skip the seeding process to avoid duplicate data.
        if (await session.Query<Product>().AnyAsync())
            return;


        // --------------------------------------------------------
        // Insert Preconfigured Products
        // --------------------------------------------------------
        // Store() performs an UPSERT operation in Marten.
        //
        // UPSERT = INSERT if record does not exist
        //        = UPDATE if record already exists
        //
        // This makes the seeding process safe to run multiple times.
        session.Store<Product>(GetPreconfiguredProducts());


        // --------------------------------------------------------
        // Persist Changes to Database
        // --------------------------------------------------------
        await session.SaveChangesAsync();
    }


    // ------------------------------------------------------------
    // GetPreconfiguredProducts
    // ------------------------------------------------------------
    // Returns a collection of sample product objects used
    // to populate the database.
    //
    // These products serve as demo/test data for the catalog.
    private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
    {
        new Product()
        {
            Id = new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"),
            Name = "IPhone X",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-1.png",
            Price = 950.00M,

            // A product can belong to multiple categories
            Category = new List<string> { "Smart Phone" }
        },

        new Product()
        {
            Id = new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"),
            Name = "Samsung 10",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-2.png",
            Price = 840.00M,
            Category = new List<string> { "Smart Phone" }
        },

        new Product()
        {
            Id = new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8"),
            Name = "Huawei Plus",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-3.png",
            Price = 650.00M,
            Category = new List<string> { "White Appliances" }
        },

        new Product()
        {
            Id = new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"),
            Name = "Xiaomi Mi 9",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-4.png",
            Price = 470.00M,
            Category = new List<string> { "White Appliances" }
        },

        new Product()
        {
            Id = new Guid("b786103d-c621-4f5a-b498-23452610f88c"),
            Name = "HTC U11+ Plus",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-5.png",
            Price = 380.00M,
            Category = new List<string> { "Smart Phone" }
        },

        new Product()
        {
            Id = new Guid("c4bbc4a2-4555-45d8-97cc-2a99b2167bff"),
            Name = "LG G7 ThinQ",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-6.png",
            Price = 240.00M,
            Category = new List<string> { "Home Kitchen" }
        },

        new Product()
        {
            Id = new Guid("93170c85-7795-489c-8e8f-7dcf3b4f4188"),
            Name = "Panasonic Lumix",
            Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageFile = "product-6.png",
            Price = 240.00M,
            Category = new List<string> { "Camera" }
        }
    };
}
