using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Core.Services;
using OrderManagement.UnitTests.Fakes;

namespace OrderManagement.UnitTests;

public class OrderServiceTests
{
    private IOrderService orderService;
    IRepository<Order> orderRepo;
    IRepository<Product> productRepo;
    IRepository<User> userRepo;

    private List<User> testUsers = new List<User>();
    private List<Product> testProducts = new List<Product>();
    private List<Order> testOrders = new List<Order>();

    public OrderServiceTests()
    {
        testUsers = new List<User>()
        {
            new User { Id = Guid.NewGuid(), Username = "User1" },
            new User { Id = Guid.NewGuid(), Username = "User2" }
        };

        testProducts = new List<Product>()
        {
            new Product() { Id = Guid.NewGuid(), Price = 10.00m, Stock = 10, Name = "Product1"},
            new Product() { Id = Guid.NewGuid(), Price = 4.99m, Stock = 7, Name = "Product2"},
        };
        
        testOrders = new List<Order>()
        {
            new Order { Id = Guid.NewGuid(), UserId = testUsers[0].Id, OrderItems = new List<OrderItem>() },
            new Order { Id = Guid.NewGuid(), UserId = testUsers[1].Id, OrderItems = new List<OrderItem>() }
        };
        
        orderRepo = new FakeOrderRepository(testOrders);
        userRepo = new FakeUserRepository(testUsers);
        productRepo = new FakeProductRepository(testProducts);
        orderService = new OrderService(orderRepo, productRepo, userRepo);
    }

    [Fact]
    public void GetAllOrders_ReturnsAllOrders()
    {
        // Act
        List<Order> result = orderService.GetAllOrders();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        
    }
    
    [Fact]
    public void GetUserOrders_InvalidUser_ThrowsException()
    {
        // Arrange
        Guid invalidUserId = Guid.NewGuid();

        // Act & Assert
        Exception ex = Assert.Throws<Exception>(() => orderService.GetUserOrders(invalidUserId));
        Assert.Equal($"User '{invalidUserId}' not found", ex.Message);
    }

    [Fact]
    public void GetOrderStatus_ValidOrder_ReturnsStatus()
    {
        // Arrange
        Order order = testOrders[0];

        // Act
        OrderStatus status = orderService.GetOrderStatus(order.Id);

        // Assert
        Assert.Equal(order.Status, status);
    }

    [Fact]
    public void GetOrderStatus_InvalidOrder_ThrowsException()
    {
        // Arrange
        Guid invalidOrderId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => orderService.GetOrderStatus(invalidOrderId));
    }

    [Fact]
    public void CreateOrder_InvalidUser_ThrowsException()
    {
        // Arrange
        Guid invalidUserId = Guid.NewGuid();
        var orderItems = new List<OrderItem> { new OrderItem { ProductId = testProducts[0].Id, Quantity = 1 } };

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => orderService.CreateOrder(invalidUserId, orderItems, DateTime.Now));
        Assert.Equal($"User {invalidUserId} not found.", ex.Message);
    }

    [Fact]
    public void CreateOrder_ProductNotFound_ThrowsException()
    {
        // Arrange
        Guid userId = testUsers[0].Id;
        Guid invalidProductId = Guid.NewGuid();
        List<OrderItem> orderItems = new List<OrderItem> { new OrderItem { ProductId = invalidProductId, Quantity = 1 } };

        // Act & Assert
        Exception ex = Assert.Throws<Exception>(() => orderService.CreateOrder(userId, orderItems, DateTime.Now));
        Assert.Equal($"Product with ID {invalidProductId} not found.", ex.Message);
    }

    [Fact]
    public void CreateOrder_InvalidQuantity_ThrowsArgumentException()
    {
        // Arrange
        Guid userId = testUsers[0].Id;
        Product product = testProducts[0];
        List<OrderItem> orderItems = new List<OrderItem> { new OrderItem { ProductId = product.Id, Quantity = 0 } };

        // Act & Assert
        Exception ex = Assert.Throws<ArgumentException>(() => orderService.CreateOrder(userId, orderItems, DateTime.Now));
        Assert.Equal($"Quantity for product '{product.Name}' must be positive.", ex.Message);
    }

    [Fact]
    public void CreateOrder_InsufficientStock_ThrowsException()
    {
        // Arrange
        Guid userId = testUsers[0].Id;
        Product product = testProducts[0];
        int requestedStock = product.Stock + 1;
        List<OrderItem> orderItems = new List<OrderItem> { new OrderItem { ProductId = product.Id, Quantity = requestedStock } };

        // Act & Assert
        Exception ex = Assert.Throws<Exception>(() => orderService.CreateOrder(userId, orderItems, DateTime.Now));
        Assert.Equal($"Not enough stock for product '{product.Name}' (requested: {requestedStock}, available: {product.Stock}).", ex.Message);
    }

    [Fact]
    public void CreateOrder_ValidOrder_StoresOrderAndUpdatesStock()
    {
        // Arrange
        Guid userId = testUsers[0].Id;
        Product product1 = testProducts[0];
        Product product2 = testProducts[1];
        List<OrderItem> orderItems = new List<OrderItem>
        {
            new OrderItem { ProductId = product1.Id, Quantity = 2 },
            new OrderItem { ProductId = product2.Id, Quantity = 3 }
        };
        int initialStock1 = product1.Stock;
        int initialStock2 = product2.Stock;
        DateTime now = new DateTime(2023, 10, 10, 15, 0, 0);

        // Act
        Order order = orderService.CreateOrder(userId, orderItems, now);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(now, order.CreatedAt);
        Assert.Equal(OrderStatus.Created, order.Status);

        Product updatedProduct1 = productRepo.Get(product1.Id);
        Product updatedProduct2 = productRepo.Get(product2.Id);
        Assert.Equal(initialStock1 - 2, updatedProduct1.Stock);
        Assert.Equal(initialStock2 - 3, updatedProduct2.Stock);

        Order savedOrder = orderRepo.Get(order.Id);
        Assert.NotNull(savedOrder);
    }

    [Fact]
    public void AdvanceOrderStatus_FromCreated_AdvancesToDelivery()
    {
        // Arrange
        Order order = testOrders[0];
        order.Status = OrderStatus.Created;
        orderRepo.Edit(order);

        // Act
        OrderStatus newStatus = orderService.AdvanceOrderStatus(order.Id);

        // Assert
        Assert.Equal(OrderStatus.Delivery, newStatus);
        Order updatedOrder = orderRepo.Get(order.Id);
        Assert.Equal(OrderStatus.Delivery, updatedOrder.Status);
        Assert.True(updatedOrder.DeliveryTime <= order.DeliveryTime);
    }

    [Fact]
    public void AdvanceOrderStatus_FromDelivery_AdvancesToDelivered()
    {
        // Arrange
        Order order = testOrders[0];
        order.Status = OrderStatus.Delivery;
        orderRepo.Edit(order);

        // Act
        OrderStatus newStatus = orderService.AdvanceOrderStatus(order.Id);

        // Assert
        Assert.Equal(OrderStatus.Delivered, newStatus);
        Order updatedOrder = orderRepo.Get(order.Id);
        Assert.Equal(OrderStatus.Delivered, updatedOrder.Status);
    }

    [Fact]
    public void AdvanceOrderStatus_AlreadyDelivered_ReturnsDelivered()
    {
        // Arrange
        Order order = testOrders[0];
        order.Status = OrderStatus.Delivered;
        orderRepo.Edit(order);

        // Act
        OrderStatus newStatus = orderService.AdvanceOrderStatus(order.Id);

        // Assert
        Assert.Equal(OrderStatus.Delivered, newStatus);
    }
    
}