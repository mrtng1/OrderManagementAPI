using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.UnitTests.Fakes;

public class FakeOrderRepository : IRepository<Order>
{
    private readonly List<Order> _orders = new();

    public bool AddWasCalled { get; private set; } = false;
    public bool EditWasCalled { get; private set; } = false;
    public bool RemoveWasCalled { get; private set; } = false;

    public FakeOrderRepository(List<Order>? seedOrders = null)
    {
        if (seedOrders != null)
        {
            _orders = new List<Order>(seedOrders);
        }
    }

    public void Add(Order order)
    {
        AddWasCalled = true;
        _orders.Add(order);
    }

    public void Edit(Order order)
    {
        EditWasCalled = true;
        var index = _orders.FindIndex(o => o.Id == order.Id);
        if (index != -1)
        {
            _orders[index] = order;
        }
    }

    public void Remove(Guid id)
    {
        RemoveWasCalled = true;
        _orders.RemoveAll(o => o.Id == id);
    }

    public Order? Get(Guid id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return _orders.ToList();
    }

    public List<Order> GetUserOrders(Guid userId)
    {
        return _orders.Where(o => o.UserId == userId).ToList();
    }
}