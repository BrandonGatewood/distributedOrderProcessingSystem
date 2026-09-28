namespace Shared.Messaging.Constants;

public static class RabbitMqConstants
{
    public const string OrderExchange = "order_exchange";
    public const string InventoryExchange = "inventory_exchange";

    public const string OrderCreatedRoutingKey = "order.created";
    public const string InventoryReservedRoutingKey = "inventory.reserved";
    public const string InventoryFailedRoutingKey = "inventory.failed";

    public const string InventoryQueue = "inventory.order-created";
    public const string OrderInventoryReservedQueue = "order.inventory-reserved";
    public const string OrderInventoryFailedQueue = "order.inventory-failed";
} 