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

    public OrderServiceTests()
    {
        var testOrders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), OrderItems = new List<OrderItem>() },
            new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), OrderItems = new List<OrderItem>() }
        };
        
        
        orderRepo = new FakeOrderRepository(testOrders);
        userRepo = new FakeUserRepository();
        productRepo = new FakeProductRepository();
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
}