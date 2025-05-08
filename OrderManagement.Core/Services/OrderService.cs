using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Core.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo;

    public OrderService(IRepository<Order> orderRepo, IRepository<Product> productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public List<Order> GetAllOrders() => _orderRepo.GetAll();

    public List<Order> GetUserOrders(Guid userId)
    {
        return _orderRepo.GetAll().Where(x => x.UserId == userId).ToList();
    }

    public OrderStatus GetOrderStatus(Guid orderId) => _orderRepo.Get(orderId).Status;

    public Order CreateOrder(Guid userId, List<OrderItem> orderItems, DateTime now)
    {
        List<Guid> productIds = orderItems.Select(x => x.ProductId).ToList();
        List<Product> products = _productRepo.GetAll().Where(x => productIds.Contains(x.Id)).ToList();
        
        foreach (OrderItem item in orderItems)
        {
            Product product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null)
                throw new Exception($"Product with ID {item.ProductId} not found.");
            
            if (item.Quantity <= 0)
            {
                throw new ArgumentException($"Quantity for product '{product.Name}' must be positive.");
            }

            if (item.Quantity > product.Stock)
                throw new Exception($"Not enough stock for product '{product.Name}' (requested: {item.Quantity}, available: {product.Stock}).");
        }
        
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
