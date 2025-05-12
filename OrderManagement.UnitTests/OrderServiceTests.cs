using Moq;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Core.Services;

namespace OrderManagement.UnitTests;

    public class OrderServiceTests
    {
        private readonly IOrderService _orderService;
        private readonly Mock<IRepository<Order>> _mockOrderRepo;
        private readonly Mock<IRepository<Product>> _mockProductRepo;
        private readonly Mock<IRepository<User>> _mockUserRepo;

        private readonly List<User> _mockUsers;
        private readonly List<Product> _mockProducts;
        private readonly List<Order> _mockOrders;

        public OrderServiceTests()
        {
            _mockUsers = new List<User>()
            {
                new User { Id = Guid.NewGuid(), Username = "User1" },
                new User { Id = Guid.NewGuid(), Username = "User2" }
            };

            _mockProducts = new List<Product>()
            {
                new Product() { Id = Guid.NewGuid(), Price = 10.00m, Stock = 10, Name = "Product1"},
                new Product() { Id = Guid.NewGuid(), Price = 4.99m, Stock = 7, Name = "Product2"},
            };

            _mockOrders = new List<Order>()
            {
                new Order { Id = Guid.NewGuid(), UserId = _mockUsers[0].Id, Status = OrderStatus.Created, OrderItems = new List<OrderItem>() },
                new Order { Id = Guid.NewGuid(), UserId = _mockUsers[0].Id, Status = OrderStatus.Delivered, OrderItems = new List<OrderItem>() },
                new Order { Id = Guid.NewGuid(), UserId = _mockUsers[1].Id, Status = OrderStatus.Delivery, OrderItems = new List<OrderItem>() }
            };

            _mockOrderRepo = new Mock<IRepository<Order>>();
            _mockProductRepo = new Mock<IRepository<Product>>();
            _mockUserRepo = new Mock<IRepository<User>>();
            _orderService = new OrderService(_mockOrderRepo.Object, _mockProductRepo.Object, _mockUserRepo.Object);
        }

        [Fact]
        public void GetAllOrders_ReturnsAllOrders()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.GetAll()).Returns(_mockOrders);

            // Act
            List<Order> result = _orderService.GetAllOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_mockOrders.Count, result.Count);
            Assert.Same(_mockOrders, result);
            _mockOrderRepo.Verify(repo => repo.GetAll(), Times.Once());
        }

        [Fact]
        public void GetUserOrders_InvalidUser_ThrowsException()
        {
            // Arrange
            Guid invalidUserId = Guid.NewGuid();
            _mockUserRepo.Setup(repo => repo.Get(invalidUserId)).Returns((User)null);

            // Act & Assert
            Exception ex = Assert.Throws<Exception>(() => _orderService.GetUserOrders(invalidUserId));
            Assert.Equal($"User '{invalidUserId}' not found", ex.Message);
            _mockUserRepo.Verify(repo => repo.Get(invalidUserId), Times.Once());
        }

        [Fact]
        public void GetUserOrders_ValidUser_ReturnsFilteredOrders()
        {
            // Arrange
            User targetUser = _mockUsers[0];
            List<Order> expectedOrdersForUser1 = _mockOrders.Where(o => o.UserId == targetUser.Id).ToList();

            _mockUserRepo.Setup(repo => repo.Get(targetUser.Id)).Returns(targetUser);
            _mockOrderRepo.Setup(repo => repo.GetAll()).Returns(_mockOrders);

            // Act
            List<Order> result = _orderService.GetUserOrders(targetUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrdersForUser1.Count, result.Count);
            Assert.All(result, order => Assert.Equal(targetUser.Id, order.UserId));
            _mockUserRepo.Verify(repo => repo.Get(targetUser.Id), Times.Once());
            _mockOrderRepo.Verify(repo => repo.GetAll(), Times.Once());
        }


        [Fact]
        public void GetOrderStatus_ValidOrder_ReturnsStatus()
        {
            // Arrange
            Order order = _mockOrders[0];
            _mockOrderRepo.Setup(repo => repo.Get(order.Id)).Returns(order);

            // Act
            OrderStatus status = _orderService.GetOrderStatus(order.Id);

            // Assert
            Assert.Equal(order.Status, status);
            _mockOrderRepo.Verify(repo => repo.Get(order.Id), Times.Once());
        }

        [Fact]
        public void GetOrderStatus_InvalidOrder_ThrowsException()
        {
            // Arrange
            Guid invalidOrderId = Guid.NewGuid();
            _mockOrderRepo.Setup(repo => repo.Get(invalidOrderId)).Returns((Order)null);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _orderService.GetOrderStatus(invalidOrderId));
            _mockOrderRepo.Verify(repo => repo.Get(invalidOrderId), Times.Once());
        }

        [Fact]
        public void CreateOrder_InvalidUser_ThrowsException()
        {
            // Arrange
            Guid invalidUserId = Guid.NewGuid();
            var orderItems = new List<OrderItem> { new OrderItem { ProductId = _mockProducts[0].Id, Quantity = 1 } };
            _mockUserRepo.Setup(repo => repo.Get(invalidUserId)).Returns((User)null);
            _mockProductRepo.Setup(repo => repo.GetAll()).Returns(_mockProducts);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _orderService.CreateOrder(invalidUserId, orderItems, DateTime.Now));
            Assert.Equal($"User {invalidUserId} not found.", ex.Message);
            _mockUserRepo.Verify(repo => repo.Get(invalidUserId), Times.Once());
            
            _mockProductRepo.Verify(repo => repo.Get(It.IsAny<Guid>()), Times.Never());
            _mockProductRepo.Verify(repo => repo.GetAll(), Times.Never());
        }

        [Fact]
        public void CreateOrder_ProductNotFound_ThrowsException()
        {
            // Arrange
            Guid userId = _mockUsers[0].Id;
            Guid invalidProductId = Guid.NewGuid();
            List<OrderItem> orderItems = new List<OrderItem> { new OrderItem { ProductId = invalidProductId, Quantity = 1 } };

            _mockUserRepo.Setup(repo => repo.Get(userId)).Returns(_mockUsers[0]);
            // Service uses GetAll to find products. invalidProductId won't be in _mockProducts.
            _mockProductRepo.Setup(repo => repo.GetAll()).Returns(_mockProducts);

            // Act & Assert
            Exception ex = Assert.Throws<Exception>(() => _orderService.CreateOrder(userId, orderItems, DateTime.Now));
            Assert.Equal($"Product with ID {invalidProductId} not found.", ex.Message);
            _mockUserRepo.Verify(repo => repo.Get(userId), Times.Once());
            _mockProductRepo.Verify(repo => repo.GetAll(), Times.Once());
            _mockProductRepo.Verify(repo => repo.Get(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void CreateOrder_InvalidQuantity_ThrowsArgumentException()
        {
            // Arrange
            Guid userId = _mockUsers[0].Id;
            Product productInTest = _mockProducts[0]; // Use a product from the main list
            List<OrderItem> orderItems = new List<OrderItem> { new OrderItem { ProductId = productInTest.Id, Quantity = 0 } };

            _mockUserRepo.Setup(repo => repo.Get(userId)).Returns(_mockUsers[0]);
            _mockProductRepo.Setup(repo => repo.GetAll()).Returns(_mockProducts);

            // Act & Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() => _orderService.CreateOrder(userId, orderItems, DateTime.Now));
            Assert.Equal($"Quantity for product '{productInTest.Name}' must be positive.", ex.Message);
            _mockUserRepo.Verify(repo => repo.Get(userId), Times.Once());
            _mockProductRepo.Verify(repo => repo.GetAll(), Times.Once());
        }

        [Fact]
        public void CreateOrder_InsufficientStock_ThrowsException()
        {
            // Arrange
            Guid userId = _mockUsers[0].Id;
            Product productInTest = _mockProducts[0];
            int requestedStock = productInTest.Stock + 1;
            List<OrderItem> orderItems = new List<OrderItem> { new OrderItem { ProductId = productInTest.Id, Quantity = requestedStock } };

            _mockUserRepo.Setup(repo => repo.Get(userId)).Returns(_mockUsers[0]);
            _mockProductRepo.Setup(repo => repo.GetAll()).Returns(_mockProducts);


            // Act & Assert
            Exception ex = Assert.Throws<Exception>(() => _orderService.CreateOrder(userId, orderItems, DateTime.Now));
            Assert.Equal($"Not enough stock for product '{productInTest.Name}' (requested: {requestedStock}, available: {productInTest.Stock}).", ex.Message);
            _mockUserRepo.Verify(repo => repo.Get(userId), Times.Once());
            _mockProductRepo.Verify(repo => repo.GetAll(), Times.Once());
        }

        [Fact]
        public void CreateOrder_ValidOrder_StoresOrderAndUpdatesStock()
        {
            // Arrange
            Guid userId = _mockUsers[0].Id;
            
            Product product1Copy = new Product { Id = _mockProducts[0].Id, Name = _mockProducts[0].Name, Price = _mockProducts[0].Price, Stock = _mockProducts[0].Stock };
            Product product2Copy = new Product { Id = _mockProducts[1].Id, Name = _mockProducts[1].Name, Price = _mockProducts[1].Price, Stock = _mockProducts[1].Stock };

            int initialStock1 = product1Copy.Stock;
            int initialStock2 = product2Copy.Stock;

            List<OrderItem> orderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = product1Copy.Id, Quantity = 2 },
                new OrderItem { ProductId = product2Copy.Id, Quantity = 3 }
            };
            DateTime now = DateTime.Now;

            _mockUserRepo.Setup(repo => repo.Get(userId)).Returns(_mockUsers[0]);
            _mockProductRepo.Setup(repo => repo.GetAll()).Returns(new List<Product> { product1Copy, product2Copy });

            _mockOrderRepo.Setup(repo => repo.Add(It.IsAny<Order>()));
            _mockProductRepo.Setup(repo => repo.Edit(It.IsAny<Product>()));


            // Act
            Order order = _orderService.CreateOrder(userId, orderItems, now);

            // Assert
            Assert.NotNull(order);
            Assert.Equal(userId, order.UserId);
            Assert.Equal(now, order.CreatedAt);
            Assert.Equal(OrderStatus.Created, order.Status);
            Assert.Equal(2, order.OrderItems.Count);
            Assert.True(order.OrderItems.Any(oi => oi.ProductId == product1Copy.Id && oi.Quantity == 2));
            Assert.True(order.OrderItems.Any(oi => oi.ProductId == product2Copy.Id && oi.Quantity == 3));

            _mockOrderRepo.Verify(repo => repo.Add(It.Is<Order>(o =>
                o.UserId == userId &&
                o.CreatedAt == now &&
                o.Status == OrderStatus.Created &&
                o.OrderItems.Count == 2 &&
                o.OrderItems.Any(oi => oi.ProductId == product1Copy.Id && oi.Quantity == 2) &&
                o.OrderItems.Any(oi => oi.ProductId == product2Copy.Id && oi.Quantity == 3)
            )), Times.Once());
        }

        [Fact]
        public void AdvanceOrderStatus_FromCreated_AdvancesToDelivery()
        {
            // Arrange
            Order order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Created, UserId = _mockUsers[0].Id };
            _mockOrderRepo.Setup(repo => repo.Get(order.Id)).Returns(order);
            _mockOrderRepo.Setup(repo => repo.Edit(It.IsAny<Order>()));

            // Act
            OrderStatus newStatus = _orderService.AdvanceOrderStatus(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Delivery, newStatus);
            _mockOrderRepo.Verify(repo => repo.Get(order.Id), Times.Once());
            _mockOrderRepo.Verify(repo => repo.Edit(It.Is<Order>(o =>
                o.Id == order.Id &&
                o.Status == OrderStatus.Delivery &&
                o.DeliveryTime != null
            )), Times.Once());
        }

        [Fact]
        public void AdvanceOrderStatus_FromDelivery_AdvancesToDelivered()
        {
            // Arrange
            Order order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Delivery, UserId = _mockUsers[0].Id, DeliveryTime = DateTime.UtcNow.AddHours(-1) };
            _mockOrderRepo.Setup(repo => repo.Get(order.Id)).Returns(order);
            _mockOrderRepo.Setup(repo => repo.Edit(It.IsAny<Order>()));

            // Act
            OrderStatus newStatus = _orderService.AdvanceOrderStatus(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Delivered, newStatus);
            _mockOrderRepo.Verify(repo => repo.Get(order.Id), Times.Once());
            _mockOrderRepo.Verify(repo => repo.Edit(It.Is<Order>(o =>
                o.Id == order.Id &&
                o.Status == OrderStatus.Delivered
            )), Times.Once());
        }

        [Fact]
        public void AdvanceOrderStatus_AlreadyDelivered_ReturnsDelivered()
        {
            // Arrange
            Order order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Delivered, UserId = _mockUsers[0].Id, DeliveryTime = DateTime.UtcNow.AddHours(-2) };
            _mockOrderRepo.Setup(repo => repo.Get(order.Id)).Returns(order);

            // Act
            OrderStatus newStatus = _orderService.AdvanceOrderStatus(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Delivered, newStatus);
            _mockOrderRepo.Verify(repo => repo.Get(order.Id), Times.Once());
            _mockOrderRepo.Verify(repo => repo.Edit(It.IsAny<Order>()), Times.Never());
        }
    }