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
                new User { Username = "Bob" },
                new User { Id = Guid.Parse("0d8e48d3-d38a-421b-9f56-fae031222fc4") ,Username = "TestUser" }
            );
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Bottles", Price = 10.99m, Stock = 5},
                new Product {  Name = "Fries", Price = 4.99m, Stock = 10},
                new Product { Id = Guid.Parse("6f129be7-ca5e-4796-8724-56429e30f40d"), Name = "TestProduct", Price = 1.99m, Stock = 10},
                new Product { Id = Guid.Parse("086b496d-b01e-44af-9102-aa9c086ae1e4"), Name = "TestProduct2", Price = 2.99m, Stock = 5}
            );
        }

        context.SaveChanges();
    }
}
