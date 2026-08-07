using DotNetEnv;
using Inventory.Worker;
using Inventory.Application.Services;
using Inventory.Application.Interfaces;
using Shared.Messaging.Configuration;
using Shared.Messaging.Interfaces;
using Shared.Messaging.RabbitMq;

Env.Load("../../../.env");

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddSingleton<IEventConsumer, EventConsumer>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

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
