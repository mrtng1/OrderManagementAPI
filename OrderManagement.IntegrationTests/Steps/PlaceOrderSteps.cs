using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OrderManagement.Core;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Core.Services;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace OrderManagement.UnitTests
{
    [Binding]
    public class CreateOrderSteps
    {
        private OrderService _orderService;
        private Exception _exception;
        private Order _response;

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
        
        [Given(@"the current time is ""(.*)""")]
        public void GivenTheCurrentTimeIs(string datetimeStr)
        {
            if (!DateTime.TryParse(datetimeStr, out var parsedDateTime))
            {
                throw new ArgumentException("Invalid datetime format.");
            }

            _currentDateTime = parsedDateTime;
        }


        [When(@"the user creates an order with the following items:")]
        public async Task WhenUserCreatesOrderWithItems(Table table)
        {
            _orderItems = table.CreateSet<OrderItem>().ToList();

            try
            {
                _response = _orderService.CreateOrder(_currentUserId, _orderItems, DateTime.Now);
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
                _response = _orderService.CreateOrder(parsedUserId, new List<OrderItem>(), DateTime.Now);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Then(@"the order should be created successfully")]
        public void ThenOrderShouldBeCreatedSuccessfully()
        {
            Assert.NotNull(_response);
            Assert.NotNull(_response.Id);
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
    }
}
