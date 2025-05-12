using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IRepository<Order>
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext context)
    {
        _db = context;
    }

    public Order? Get(Guid id)
    {
        return _db.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return _db.Orders
            .Include(x => x.OrderItems)
            .ToList();
    }

    public List<Order> GetUserOrders(Guid userId)
    {
        return _db.Orders
            .Where(o => o.UserId == userId)
            .ToList();
    }

    public void Add(Order order)
    {
        _db.Orders.Add(order);
        _db.SaveChanges();
    }

    public void Edit(Order order)
    {
        _db.Orders.Update(order);
        _db.SaveChanges();
    }
}