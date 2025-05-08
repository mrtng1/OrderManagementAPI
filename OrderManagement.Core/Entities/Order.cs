using System.Text.Json.Serialization;

namespace OrderManagement.Core.Entities;

public enum OrderStatus
{
    Created,
    Delivery,
    Delivered
}

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime DeliveryTime { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
}

public class OrderItem
{
    [JsonIgnore]
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonIgnore]
    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Order? Order { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}