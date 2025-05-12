using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Core.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<User> _userRepo;

    public OrderService(IRepository<Order> orderRepo, IRepository<Product> productRepo, IRepository<User> userRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public List<Order> GetAllOrders() => _orderRepo.GetAll();

    public List<Order> GetUserOrders(Guid userId)
    {
        User checkUser = _userRepo.Get(userId);
        if (checkUser == null)
            throw new Exception($"User '{userId}' not found");
        
        return _orderRepo.GetAll().Where(x => x.UserId == userId).ToList();
    }

    public OrderStatus GetOrderStatus(Guid orderId) => _orderRepo.Get(orderId).Status;
    public DateTime GetOrderDeliveryTime(Guid orderId) => _orderRepo.Get(orderId).DeliveryTime;

    public Order CreateOrder(Guid userId, List<OrderItem> orderItems, DateTime now)
    {
        User checkUser = _userRepo.Get(userId);
        if (checkUser == null)
            throw new Exception($"User {userId} not found.");
        
        List<Guid> productIds = orderItems.Select(x => x.ProductId).ToList();
        List<Product> products = _productRepo.GetAll().Where(x => productIds.Contains(x.Id)).ToList();
        
        foreach (OrderItem orderItem in orderItems)
        {
            Product? product = products.FirstOrDefault(p => p.Id == orderItem.ProductId);
            if (product == null)
                throw new Exception($"Product with ID {orderItem.ProductId} not found.");
            
            if (orderItem.Quantity <= 0)
                throw new ArgumentException($"Quantity for product '{product.Name}' must be positive.");

            if (orderItem.Quantity > product.Stock)
                throw new Exception($"Not enough stock for product '{product.Name}' (requested: {orderItem.Quantity}, available: {product.Stock}).");

            product.Stock -= orderItem.Quantity;
        }
        
        Order order = new Order
        {
            UserId = userId,
            OrderItems = orderItems,
            CreatedAt = now,
            Status = OrderStatus.Created,
            DeliveryTime = CalculateDeliveryTime(now, OrderStatus.Created),
        };

        _orderRepo.Add(order);
        return order;
    }

    public OrderStatus AdvanceOrderStatus(Guid orderId)
    {
        Order order = _orderRepo.Get(orderId);
        if (order == null)
            throw new Exception($"Order '{orderId}' not found.");
            
        if (order.Status == OrderStatus.Delivered) 
            return order.Status;

        order.Status++;
        order.DeliveryTime = CalculateDeliveryTime(DateTime.Now, order.Status);
        _orderRepo.Edit(order);
        return order.Status;
    }

    private bool IsWeekend(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    
    private DateTime CalculateDeliveryTime(DateTime originalTime, OrderStatus status)
    {
        DateTime result = originalTime;

        int daysToAdd = status switch
        {
            OrderStatus.Created => 2,
            OrderStatus.Delivery => 1,
            _ => 0
        };

        // Add Days & skip weekends
        while (daysToAdd > 0)
        {
            result = result.AddDays(1);
            if (!IsWeekend(result))
            {
                daysToAdd--;
            }
        }
        
        // Ordered after closing time
        if (result.Hour >= 16 && !IsWeekend(originalTime))
        {
            result = result.AddDays(1);
        }

        return result;
    }
}
