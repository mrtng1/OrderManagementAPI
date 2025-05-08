using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public IActionResult CreateOrder(Guid userId, [FromBody] List<OrderItem> orderItems)
    {
        var order = _orderService.CreateOrder(userId, orderItems, DateTime.UtcNow);
        return Ok(order);
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_orderService.GetAllOrders());
    }

    [HttpPost("{id}/advance")]
    public IActionResult AdvanceOrder(Guid id)
    {
        _orderService.AdvanceOrderStatus(id);
        return NoContent();
    }
    
    [HttpGet("by-user/{userId}")]
    public IActionResult GetUserOrders(Guid userId)
    {
        return Ok(_orderService.GetUserOrders(userId));
    }
    
    [HttpGet("status/{id}")]
    public IActionResult GetOrderStatus(Guid id)
    {
        return Ok(_orderService.GetOrderStatus(id));
    }
    
}
