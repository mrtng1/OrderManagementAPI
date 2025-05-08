using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Infrastructure.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly AppDbContext db;

    public ProductRepository(AppDbContext context)
    {
        db = context;
    }
    
    public Product? Get(Guid id)
    {
        return db.Products
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Product> GetAll()
    {
        return db.Products
            .ToList();
    }
    

    public void Add(Product order)
    {
        db.Products.Add(order);
        db.SaveChanges();
    }

    public void Edit(Product order)
    {
        db.Products.Update(order);
        db.SaveChanges();
    }
}