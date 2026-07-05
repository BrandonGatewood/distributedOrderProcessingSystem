using OrderService.Application.Interfaces;
using OrderService.Application.Services;

var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.IsDevelopment())
{
    //Env.load();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IOrderApplicationService, OrderApplicationService>();

var app = builder.Build();
app.MapControllers();
app.Run();
