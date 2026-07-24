# distributedOrderProcessingSystem

A distributed order processing system built to explore microservices architecture, 
event-driven communication, and asynchronous messaging using RabbitMQ.

The goal of this project is to learn how independent services communicate through 
a message broker and handle distributed workflows.

## Tech Stack

- .NET
- RabbitMQ
- PostgreSQL
- Docker

---

# Architecture

The system uses an event-driven microservice architecture.

## Current Workflow

```mermaid
flowchart TD
    Client --> OrderService
    OrderService --> DB[(Order Database)]
    OrderService --> RabbitMQ

    RabbitMQ --> InventoryService

    InventoryService -->|Inventory Available| RabbitMQ
    InventoryService -->|Inventory Unavailable| RabbitMQ

    RabbitMQ --> OrderService

    OrderService -->|Completed| CompletedOrder[Completed]
    OrderService -->|Failed| FailedOrder[Failed]
```

Services communicate asynchronously through RabbitMQ events rather than direct service-to-service calls.

---

## Order Service

The Order Service is responsible for creating orders and publishing order events.

Flow:

1. Receive order request
2. Create order with `Pending` status
3. Save order to database
4. Publish `OrderCreated` event to RabbitMQ

Example event:

```json
{
   "userId": "8b3f1d7e-6a9a-4d9f-bb8f-8c6b8f4c2e11",
   "orderItems": [
      {
         "productId": "2a5c9e8f-7c41-4e2a-9b52-4d1e8c3f9012",
         "productName": "Mechanical Keyboard",
         "unitPrice": 89.99,
         "quantity": 1
      },
      {
         "productId": "6f9e2b44-3a1d-4c6b-a6d7-5e8f90123456",
         "productName": "Wireless Mouse",
         "unitPrice": 29.99,
         "quantity": 2
      }
   ]
}
```

## Inventory Service

The Inventory Service will consume OrderCreated events and verify inventory availability.

Flow:

OrderCreated Event
        |
        v
Inventory Service
        |
        +--> Available
        |        |
        |        v
        |   Order Completed
        |
        +--> Unavailable
                 |
                 v
            Order Failed

## Order Status Flow

Pending
   |
   +---- Completed
   |
   +---- Failed

## Future Improvements

Inventory Service

Payment Service

Notification Service

Retry handling

Dead-letter queues

Idempotent message processing

Distributed tracing

Docker Compose setup
