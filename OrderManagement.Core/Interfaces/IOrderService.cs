using OrderManagement.Core.Entities;

namespace OrderManagement.Core.Interfaces;

public interface IOrderService
{
    List<Order> GetAllOrders();
    List<Order> GetUserOrders(Guid userId);
    OrderStatus GetOrderStatus(Guid orderId);
    DateTime GetOrderDeliveryTime(Guid orderId);
    Order CreateOrder(Guid userId, List<OrderItem> orderItems, DateTime now);
    OrderStatus AdvanceOrderStatus(Guid orderId);
}