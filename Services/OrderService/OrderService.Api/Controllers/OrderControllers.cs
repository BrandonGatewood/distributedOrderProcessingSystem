using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Requests;
using OrderService.Application.Interfaces;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/order")]
public class OrderControllers(IOrderApplicationService orderApplicationService) : ControllerBase
{
    private readonly IOrderApplicationService _orderService = orderApplicationService;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {

        return Ok(await _orderService.CreateOrderAsync(request));
    }

    [HttpGet("{orderId}/status")]
    public async Task<IActionResult> GetOrderStatus(Guid orderId)
    {
        var response = await _orderService.GetOrderStatusAsync(orderId);

        if (response is null)
            return NotFound();

        return Ok(response);
    } 
}