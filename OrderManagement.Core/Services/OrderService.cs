using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Core.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;

    public OrderService(IRepository<Order> orderRepo)
    {
        _orderRepo = orderRepo;
    }

    public List<Order> GetAllOrders() => _orderRepo.GetAll();

    public List<Order> GetUserOrders(Guid userId)
    {
        return _orderRepo.GetAll().Where(x => x.UserId == userId).ToList();
    }

    public OrderStatus GetOrderStatus(Guid orderId) => _orderRepo.Get(orderId).Status;

    public Order CreateOrder(Guid userId, List<OrderItem> orderItems, DateTime now)
    {
        // get list of relevant products
        // check if their stock is 
        Order order = new Order
        {
            UserId = userId,
            OrderItems = orderItems,
            CreatedAt = now,
            Status = IsWeekend(now) ? OrderStatus.Created : OrderStatus.Processed
        };

        _orderRepo.Add(order);
        return order;
    }

    public void AdvanceOrderStatus(Guid orderId)
    {
        Order order = _orderRepo.Get(orderId);
        if (order == null) return;

        if (order.Status == OrderStatus.Delivered) return;

        order.Status++;
        _orderRepo.Edit(order);
    }

    private bool IsWeekend(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
}
