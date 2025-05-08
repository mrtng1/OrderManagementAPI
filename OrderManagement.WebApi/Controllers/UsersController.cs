using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_userService.GetAllUsers());
    }
    
    [HttpPost]
    public IActionResult CreateUser(string username)
    {
        return Ok(_userService.CreateUser(username));
    }
}