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
        if (userId == Guid.Empty)
        {
            return BadRequest(new { message = "User ID cannot be empty." });
        }

        if (orderItems == null || !orderItems.Any())
        {
            return BadRequest(new { message = "Order items cannot be empty." });
        }
        
        try
        {
            Order order = _orderService.CreateOrder(userId, orderItems, DateTime.UtcNow);
            return Ok(new { message = "Order created successfully.", orderId = order.Id, deliveryTime = order.DeliveryTime });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex) 
        {
            if (ex.Message.Contains("Product with ID") && ex.Message.Contains("not found"))
            {
                return NotFound(new { message = ex.Message });
            }
            else if (ex.Message.Contains("Not enough stock for product"))
            {
                return BadRequest(new { message = ex.Message });
            }
            
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
        
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_orderService.GetAllOrders());
    }
    
    /// <summary>
    /// Order/Package gets scanned and advances to the next status.
    /// Created -> Delivery -> Delivered
    /// </summary>
    /// <param name="id">Order id</param>
    /// <returns></returns>
    [HttpPut("{id}/scan-order")]
    public IActionResult ScanOrder(Guid id)
    {
        try
        {
            OrderStatus newStatus = _orderService.AdvanceOrderStatus(id);
            return Ok(new { message = "Package set to delivery.", newStatus = newStatus.ToString() });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    [HttpGet("by-user/{userId}")]
    public IActionResult GetUserOrders(Guid userId)
    {
        try
        {
            return Ok(_orderService.GetUserOrders(userId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("status/{id}")]
    public IActionResult GetOrderStatus(Guid id)
    {
        OrderStatus status = _orderService.GetOrderStatus(id);
        DateTime expectedDeliveryTime = _orderService.GetOrderDeliveryTime(id);
        
        return Ok(new { status = status.ToString(), expectedDeliveryTime });
    }
    
}
