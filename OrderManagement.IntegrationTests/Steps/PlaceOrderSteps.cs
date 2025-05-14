using Moq;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Core.Services;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace OrderManagement.UnitTests;

[Binding]
public class CreateOrderSteps
{
    private OrderService _orderService;
    private Exception _exception;
    private Order _createdOrder;

    private Mock<IRepository<User>> _mockUserRepo;
    private Mock<IRepository<Product>> _mockProductRepo;
    private Mock<IRepository<Order>> _mockOrderRepo;

    private List<User> _users;
    private List<Product> _products;
    private List<Order> _orders;

    private Guid _currentUserId;
    private DateTime _currentDateTime;
    private List<OrderItem> _orderItems;
    private Dictionary<Guid, int> _initialStock;

    [BeforeScenario]
    public void Setup()
    {
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockProductRepo = new Mock<IRepository<Product>>();
        _mockOrderRepo = new Mock<IRepository<Order>>();

        _users = new List<User>();
        _products = new List<Product>();
        _orders = new List<Order>();
        _orderItems = new List<OrderItem>();
        _initialStock = new Dictionary<Guid, int>();
        
        _currentDateTime = DateTime.Now;

        _mockUserRepo.Setup(r => r.Get(It.IsAny<Guid>()))
            .Returns((Guid id) => _users.FirstOrDefault(u => u.Id == id));

        _mockProductRepo.Setup(r => r.Get(It.IsAny<Guid>()))
            .Returns((Guid id) => _products.FirstOrDefault(p => p.Id == id));

        _mockProductRepo.Setup(r => r.GetAll())
            .Returns(() => _products);

        _mockOrderRepo.Setup(r => r.Add(It.IsAny<Order>()))
            .Callback<Order>(order => _orders.Add(order));

        _orderService = new OrderService(_mockOrderRepo.Object, _mockProductRepo.Object, _mockUserRepo.Object);
    }

    [Given(@"a user exists with ID ""(.*)""")]
    public void GivenUserExists(Guid userId)
    {
        _currentUserId = userId;
        _users.Add(new User { Id = userId, Username = "testuser" });
    }

    [Given(@"the following products exist with stock:")]
    public void GivenProductsExistWithStock(Table table)
    {
        foreach (var row in table.Rows)
        {
            var product = new Product
            {
                Id = Guid.Parse(row["ProductId"]),
                Name = row["Name"],
                Stock = int.Parse(row["Stock"])
            };

            _products.Add(product);
            _initialStock[product.Id] = product.Stock;
        }
    }
    
    [Given(@"the current time is ""(.*)"" during a random weekday")]
    public void GivenTheCurrentTimeIsDuringWeekday(string datetimeStr)
    {
        if (!DateTime.TryParse(datetimeStr, out var parsedTime))
        {
            throw new ArgumentException("Invalid datetime format.");
        }

        var today = DateTime.Now.Date;

        // If weekend, shift to upcoming Monday
        if (today.DayOfWeek == DayOfWeek.Saturday)
            today = today.AddDays(2);
        else if (today.DayOfWeek == DayOfWeek.Sunday)
            today = today.AddDays(1);

        _currentDateTime = new DateTime(
            today.Year,
            today.Month,
            today.Day,
            parsedTime.Hour,
            parsedTime.Minute,
            parsedTime.Second
        );
    }
    
    [Given(@"the current time is ""(.*)"" during a weekend")]
    public void GivenTheCurrentTimeIsDuringWeekend(string datetimeStr)
    {
        if (!DateTime.TryParse(datetimeStr, out var parsedTime))
        {
            throw new ArgumentException("Invalid datetime format.");
        }

        DateTime today = DateTime.Now.Date;

        DateTime daysUntilWeekend = Enumerable.Range(0, 7)
            .Select(offset => today.AddDays(offset))
            .First(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);

        _currentDateTime = new DateTime(
            daysUntilWeekend.Year,
            daysUntilWeekend.Month,
            daysUntilWeekend.Day,
            parsedTime.Hour,
            parsedTime.Minute,
            parsedTime.Second
        );
    }

    
    [Given(@"the current time is ""(.*)"" (.*) days from today")]
    public void GivenTheCurrentTimeIsDuringWeekdayAndDaysFromToday(string datetimeStr, int daysFromToday)
    {
        if (!DateTime.TryParse(datetimeStr, out var parsedTime))
        {
            throw new ArgumentException("Invalid datetime format.");
        }

        DateTime targetDate = DateTime.Now.Date.AddDays(daysFromToday);

        _currentDateTime = new DateTime(
            targetDate.Year,
            targetDate.Month,
            targetDate.Day,
            parsedTime.Hour,
            parsedTime.Minute,
            parsedTime.Second
        );
    }


    [When(@"the user creates an order with the following items:")]
    public async Task WhenUserCreatesOrderWithItems(Table table)
    {
        _orderItems = table.CreateSet<OrderItem>().ToList();

        try
        {
            _createdOrder = _orderService.CreateOrder(_currentUserId, _orderItems, _currentDateTime);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }
    
    [When(@"an order is created with user ID ""(.*)""")]
    public void WhenAnOrderIsCreatedWithUserID(string userId)
    {
        try
        {
            Guid parsedUserId = Guid.Parse(userId);
            _createdOrder = _orderService.CreateOrder(parsedUserId, new List<OrderItem>(), DateTime.Now);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the order should be created successfully")]
    public void ThenOrderShouldBeCreatedSuccessfully()
    {
        Assert.NotNull(_createdOrder);
        Assert.NotNull(_createdOrder.Id);
        Assert.Null(_exception);
    }

    [Then(@"an error ""(.*)"" should be thrown")]
    public void ThenErrorShouldBeThrown(string errorType)
    {
        Assert.NotNull(_exception);
        Assert.Equal(errorType, _exception.Message);
    }

    [Then(@"the stock should be updated accordingly")]
    public void ThenTheStockShouldBeUpdatedAccordingly()
    {
        foreach (var item in _orderItems)
        {
            var product = _products.FirstOrDefault(p => p.Id == item.ProductId);
            Assert.NotNull(product);
            var initial = _initialStock[item.ProductId];
            var expected = initial - item.Quantity;
            Assert.Equal(expected, product.Stock);
        }
    }
    
    [Then(@"the delivery date should be (.*) working days after")]
    public void ThenTheDeliveryDateShouldBeXWorkingDaysAfter(int daysLater)
    {
        DateTime expectedDate = AddWorkingDays(_currentDateTime, daysLater);
        Assert.Equal(expectedDate, _createdOrder.DeliveryTime);
    }
    
    [Then(@"the delivery date should skip Saturday and Sunday")]
    public void DeliveryDateShouldSkipWeekend()
    {
        Assert.NotEqual(DayOfWeek.Saturday, _createdOrder.DeliveryTime.DayOfWeek);
        Assert.NotEqual(DayOfWeek.Sunday, _createdOrder.DeliveryTime.DayOfWeek);
    }

    
    // Helper
    private DateTime AddWorkingDays(DateTime startDate, int workingDays)
    {
        DateTime result = startDate;
        while (workingDays > 0)
        {
            result = result.AddDays(1);
            if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays--;
            }
        }
        return result;
    }
}
    