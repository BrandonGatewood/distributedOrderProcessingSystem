using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Application.Services;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Repositories;


Env.Load("../../../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Db Configuration
var connectionString = builder.Configuration["ORDERSERVICEDB:CONNECTIONSTRING"]
    ?? throw new InvalidOperationException("Order Service Database connection string is missing in configuration.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderApplicationService, OrderApplicationService>();
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3);
    });
});

builder.Services
    .AddOptions<RabbitMqSettings>()
    .Bind(builder.Configuration.GetSection("RabbitMQ"))
    .Validate(settings =>
    {
        return !string.IsNullOrEmpty(settings.Host)
            && !string.IsNullOrEmpty(settings.Username)
            && !string.IsNullOrEmpty(settings.Password);
    }, "RabbitMQ configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>();

var app = builder.Build();
app.MapControllers();
app.Run();
