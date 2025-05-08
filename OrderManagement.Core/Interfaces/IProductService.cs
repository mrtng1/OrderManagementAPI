using OrderManagement.Core.Entities;

namespace OrderManagement.Core.Interfaces;

public interface IProductService
{
    List<Product> GetAllProducts();
    Product CreateProduct(string name, decimal price, int stock);
    Product GetProduct(Guid id);
}