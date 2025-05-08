using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IRepository<Order>
{
    private readonly AppDbContext db;

    public OrderRepository(AppDbContext context)
    {
        db = context;
    }

    public Order? Get(Guid id)
    {
        return db.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return db.Orders
            .Include(x => x.OrderItems)
            .ToList();
    }

    public List<Order> GetUserOrders(Guid userId)
    {
        return db.Orders
            .Where(o => o.UserId == userId)
            .ToList();
    }

    public void Add(Order order)
    {
        db.Orders.Add(order);
        db.SaveChanges();
    }

    public void Edit(Order order)
    {
        db.Orders.Update(order);
        db.SaveChanges();
    }
}