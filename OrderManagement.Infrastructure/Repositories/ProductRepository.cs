using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Infrastructure.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext context)
    {
        _db = context;
    }
    
    public Product? Get(Guid id)
    {
        return _db.Products
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Product> GetAll()
    {
        return _db.Products
            .ToList();
    }
    

    public void Add(Product product)
    {
        _db.Products.Add(product);
        _db.SaveChanges();
    }

    public void Edit(Product product)
    {
        _db.Products.Update(product);
        _db.SaveChanges();
    }
}