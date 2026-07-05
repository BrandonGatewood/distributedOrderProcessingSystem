using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Requests;
using OrderService.Application.Interfaces;
namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/order")]
public class OrderControllers(IOrderApplicationService orderService) : ControllerBase
{
    private readonly IOrderApplicationService _orderService = orderService;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {

        return Ok(_orderService.CreateOrderAsync(request));
    }
}