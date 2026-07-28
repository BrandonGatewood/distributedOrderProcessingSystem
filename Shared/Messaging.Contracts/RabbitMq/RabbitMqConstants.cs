namespace Messaging.Contracts.RabbitMq;

public static class RabbitMqConstants
{
    public const string OrderExchange = "order_exchange";
    public const string InventoryReservedExchange = "inventory_reserved_exchange";
    public const string InventoryFailedExchange = "inventory_failed_exchange";
    public const string OrderCreatedRoutingKey = "order.created";
    public const string InventoryReservdRoutingKey = "inventory.reserved";
    public const string InventoryFailedRoutingKey = "inventory.failed";
} 