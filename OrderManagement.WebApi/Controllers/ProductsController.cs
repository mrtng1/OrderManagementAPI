using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public IActionResult CreateProduct(string productName, decimal price, int stock)
    {
        var order = _productService.CreateProduct(productName, price, stock);
        return Ok(order);
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_productService.GetAllProducts());
    }
}