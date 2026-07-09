using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;
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
}