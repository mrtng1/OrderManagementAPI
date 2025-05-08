using OrderManagement.Core.Entities;

namespace OrderManagement.Infrastructure;

public class DbInitializer : IDbInitializer
{
    public void Initialize(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        if (context.Orders.Any())
        {
            return;   // DB has been seeded
        }
        
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Username = "Alice" },
                new User { Username = "Bob" }
            );
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                //new Product { Id = Guid.Parse("086b496d-b01e-44af-9102-aa9c086ae1e4"), Name = "Bottles", Price = 10.99m },
                //new Product { Id = Guid.Parse("6f129be7-ca5e-4796-8724-56429e30f40d"), Name = "Fries", Price = 4.99m },
                new Product { Name = "Bottles", Price = 10.99m, Stock = 5},
                new Product {  Name = "Fries", Price = 4.99m, Stock = 10}
            );
        }

        context.SaveChanges();
    }
}
