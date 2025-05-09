using System.Data;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Core.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepo;

    public ProductService(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }
    
    public List<Product> GetAllProducts() => _productRepo.GetAll();
    public Product GetProduct(Guid id) => _productRepo.Get(id);
    
    public Product CreateProduct(string productName, decimal price, int stock)
    {
        Product checkProduct = _productRepo.GetAll().FirstOrDefault(x => x.Name == productName);
        if (checkProduct != null)
            throw new DuplicateNameException($"Username {productName} already taken.");
        
        Product product = new Product
        {
            Name = productName,
            Price = price,
            Stock = stock
        };

        _productRepo.Add(product);
        return product;
    }
}