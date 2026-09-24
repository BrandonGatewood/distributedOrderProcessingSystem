using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Inventory.Worker;
using Inventory.Application.Services;
using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Data;
using Shared.Messaging.Configuration;
using Shared.Messaging.Interfaces;
using Shared.Messaging.RabbitMq;

Env.Load("../../../.env");

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();
// Db Configuration
var connectionString = builder.Configuration["INVENTORYSERVICEDB:CONNECTIONSTRING"]
    ?? throw new InvalidOperationException("Inventory Service Database connection string is missing in configuration.");

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddSingleton<IEventConsumer, EventConsumer>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddDbContext<InventoryDbContext>(options =>
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

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
