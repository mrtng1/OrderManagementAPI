using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.UnitTests.Fakes;

public class FakeProductRepository : IRepository<Product>
{
    private readonly List<Product> _products = new();

    public bool AddWasCalled { get; private set; } = false;
    public bool EditWasCalled { get; private set; } = false;
    public bool RemoveWasCalled { get; private set; } = false;

    public FakeProductRepository(List<Product>? seedProducts = null)
    {
        if (seedProducts != null)
        {
            _products = new List<Product>(seedProducts);
        }
    }

    public Product? Get(Guid id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public List<Product> GetAll()
    {
        return _products.ToList(); 
    }

    public void Add(Product product)
    {
        AddWasCalled = true;
        _products.Add(product);
    }

    public void Edit(Product product)
    {
        EditWasCalled = true;
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            _products[index] = product;
        }
    }

    public void Remove(Guid id)
    {
        RemoveWasCalled = true;
        _products.RemoveAll(p => p.Id == id);
    }
}