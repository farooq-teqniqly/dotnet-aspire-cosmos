# Getting Up to Speed with Cosmos DB (for ASP.NET Core + RabbitMQ Developers)

This document consolidates guidance on **Azure Cosmos DB** design and tradeoffs  
for developers already familiar with **ASP.NET Core** and **RabbitMQ**.  
It focuses on high-throughput, event-driven SaaS workloads where data modeling,  
partitioning, and change-feed integration matter most.

---

## Table of Contents

- [Getting Up to Speed with Cosmos DB (for ASP.NET Core + RabbitMQ Developers)](#getting-up-to-speed-with-cosmos-db-for-aspnet-core--rabbitmq-developers)
  - [Table of Contents](#table-of-contents)
  - [1. Core Mental Model](#1-core-mental-model)
    - [Resources \& Readings](#resources--readings)
    - [Concepts](#concepts)
  - [2. Data Modeling Patterns](#2-data-modeling-patterns)
    - [Guides](#guides)
    - [a) Single-Document Aggregates](#a-single-document-aggregates)
    - [b) Multiple Types per Container](#b-multiple-types-per-container)
    - [c) TransactionalBatch](#c-transactionalbatch)
  - [3. Change Feed: Cosmos → RabbitMQ Bridge](#3-change-feed-cosmos--rabbitmq-bridge)
    - [Docs](#docs)
    - [Pattern](#pattern)
    - [Tradeoffs](#tradeoffs)
  - [4. RU, Throttling, and Resiliency](#4-ru-throttling-and-resiliency)
    - [References](#references)
    - [Practices](#practices)
  - [5. Partition Keys and Hot Partitions](#5-partition-keys-and-hot-partitions)
    - [Read](#read)
    - [Heuristics](#heuristics)
  - [6. Cost and Throughput Planning](#6-cost-and-throughput-planning)
    - [Cost Planning Docs](#cost-planning-docs)
    - [Tips](#tips)
  - [7. EF Core Integration (Pros/Cons)](#7-ef-core-integration-proscons)
    - [EF Core References](#ef-core-references)
    - [Pros](#pros)
    - [Cons](#cons)
    - [Guidance](#guidance)
  - [8. Practical 5-Day Study Plan](#8-practical-5-day-study-plan)
  - [9. Recommended Learning Resources](#9-recommended-learning-resources)
  - [10. Key Takeaways](#10-key-takeaways)

---

## 1. Core Mental Model

### Resources & Readings

- [Request Units and Throughput](https://learn.microsoft.com/azure/cosmos-db/request-units)  
- [Partitioning in Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/partitioning-overview)  
- [Consistency Levels](https://learn.microsoft.com/azure/cosmos-db/consistency-levels)

### Concepts

- Every operation costs **Request Units (RUs)**; they represent CPU + IO + memory.
- **Containers** ≠ tables; **items** ≠ rows. Each item lives in a **logical partition** determined by a `partitionKey`.
- **Cross-partition operations** are expensive—avoid them where possible.
- Most workloads run well on **Session Consistency**, balancing correctness and latency.

**Action Item**  
Define your **partition key** before writing any code. Everything—performance, cost, and atomicity—depends on that choice.

---

## 2. Data Modeling Patterns

### Guides

- [Data modeling in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/modeling-data)  
- [Choosing the right partition key](https://learn.microsoft.com/azure/cosmos-db/partitioning-overview#choose-partitionkey)

### a) Single-Document Aggregates

- Store related entities (e.g., order + lines) as one JSON document.  
- **Pros:** Single read/write, simple transactions.  
- **Cons:** Document size growth; concurrency applies to entire aggregate.

### b) Multiple Types per Container

- Use a discriminator field (`type`) to mix entities sharing a partition key:

  ```json
  { "id": "order-9843", "type": "order", "customerId": "1234", "total": 199.99 }
  ```

- Query efficiently:

  ```sql
  SELECT * FROM c WHERE c.customerId = "1234"
  ```

### c) TransactionalBatch

- Enables multi-document atomic operations **within one partition**:

  ```csharp
  var batch = container.CreateTransactionalBatch(new PartitionKey(customerId));
  batch.CreateItem(order);
  batch.CreateItem(orderAudit);
  await batch.ExecuteAsync();
  ```

---

## 3. Change Feed: Cosmos → RabbitMQ Bridge

### Docs

- [Change feed overview](https://learn.microsoft.com/azure/cosmos-db/change-feed)  
- [Change Feed Processor in .NET](https://learn.microsoft.com/azure/cosmos-db/change-feed-processor-dotnet)

### Pattern

1. API writes to Cosmos.
2. A background **Change Feed Processor** streams changes.
3. Each change → transform → publish to RabbitMQ.

```csharp
var processor = container
    .GetChangeFeedProcessorBuilder<Order>("orders-to-bus", async (docs, ct) =>
    {
        foreach (var doc in docs)
            await rabbitPublisher.PublishAsync(MapToEvent(doc), ct);
    })
    .WithLeaseContainer(leaseContainer)
    .Build();

await processor.StartAsync();
```

### Tradeoffs

- **At-least-once** delivery → design idempotent consumers.
- **Ordering** only guaranteed per partition key.
- Backpressure handled by the worker, not the HTTP request path.

---

## 4. RU, Throttling, and Resiliency

### References

- [Handle 429 Request Rate Too Large](https://learn.microsoft.com/azure/cosmos-db/troubleshoot-request-rate-too-large)  
- [Retry policies in Cosmos DB SDK](https://learn.microsoft.com/azure/cosmos-db/nosql/sdk-dotnet-v3#sdk-configuration)

### Practices

- Log RU charge via `response.RequestCharge`.
- Expose metrics (container, operation type, partition scope).
- Treat 429s as a capacity signal—scale RUs or fix hot partitions.
- Do not stack extra retries on top of SDK’s exponential backoff.

---

## 5. Partition Keys and Hot Partitions

### Read

- [Partition key design patterns](https://learn.microsoft.com/azure/cosmos-db/partitioning-overview#design-patterns)

### Heuristics

- Choose high-cardinality key that co-locates logically related data.
- Avoid timestamp-based keys—cause “hot now” partitions.  
- Examples:
  - SaaS: `/tenantId`
  - IoT: `/deviceId`
  - Bad: `/orderId`, `/createdAt`

Be able to state:  
> “We partition by X because it keeps all related docs together **and** spreads writes evenly.”

---

## 6. Cost and Throughput Planning

### Cost Planning Docs

- [Autoscale vs Manual Throughput](https://learn.microsoft.com/azure/cosmos-db/provision-throughput-autoscale)  
- [Estimate RUs and cost](https://learn.microsoft.com/azure/cosmos-db/estimate-ru-cost)

### Tips

- Prefer **point reads** (`ReadItemAsync`) for known `id` + `partitionKey`.
- Use **autoscale** for spiky workloads.  
- Cross-partition queries and large documents multiply RU cost.  
- Profile RU usage early using Azure Metrics Explorer.

---

## 7. EF Core Integration (Pros/Cons)

### EF Core References

- [EF Core provider for Cosmos DB](https://learn.microsoft.com/ef/core/providers/cosmos/)  
- [Limitations and best practices](https://learn.microsoft.com/ef/core/providers/cosmos/limitations)

### Pros

- Familiar DbContext pattern and LINQ queries.
- Familiar DbContext pattern and LINQ queries.

### Cons

- Hides partitioning details and RU costs.
- Cross-partition scans easy to trigger unintentionally.
- Polymorphic/multi-type containers awkward.

### Guidance

Use the **Cosmos SDK** directly for latency-sensitive or  
high-throughput code paths.
Reserve EF Core for ancillary, low-volume operations.

---

## 8. Practical 5-Day Study Plan

| Day | Focus | Deliverable |
|-----|--------|-------------|
| **1–2** | Study partitioning, RU, and consistency docs. | Draft partition-key strategy for one domain. |
| **3** | Build `POST /orders` endpoint writing to Cosmos via SDK. | Log RU cost and measure latency. |
| **4** | Add Change Feed Processor → RabbitMQ publisher. | Ensure idempotent publish logic. |
| **5** | Load-test one “hot tenant” scenario. | Observe throttling, adjust partition key & RU settings. |

---

## 9. Recommended Learning Resources

| Category | Resource | Link |
|-----------|-----------|------|
| **Concepts** | Cosmos DB fundamentals | [Microsoft Learn: Introduction to Azure Cosmos DB](https://learn.microsoft.com/training/modules/introduction-to-azure-cosmos-db/) |
|  | Partitioning & scaling | [Partitioning overview](https://learn.microsoft.com/azure/cosmos-db/partitioning-overview) |
|  | RU and throughput | [Request units and cost model](https://learn.microsoft.com/azure/cosmos-db/request-units) |
| **Development** | .NET SDK quickstart | [Quickstart: .NET SDK](https://learn.microsoft.com/azure/cosmos-db/nosql/sdk-dotnet-v3) |
|  | Change Feed Processor | [Change feed patterns](https://learn.microsoft.com/azure/cosmos-db/change-feed-processor-dotnet) |
|  | TransactionalBatch | [TransactionalBatch in .NET](https://learn.microsoft.com/azure/cosmos-db/nosql/sdk-dotnet-v3#transactional-batch) |
| **Operations** | Emulator | [Cosmos DB Emulator](https://learn.microsoft.com/azure/cosmos-db/local-emulator) |
|  | Troubleshooting 429s | [Request rate too large errors](https://learn.microsoft.com/azure/cosmos-db/troubleshoot-request-rate-too-large) |
|  | Cost optimization | [Optimize cost and performance](https://learn.microsoft.com/azure/cosmos-db/optimize-cost-performance) |
| **Advanced Reading** | *Designing Data-Intensive Applications* (Kleppmann) | [Book link](https://dataintensive.net/) |
|  | *Use the Index, Luke!* (Winand) | [Book site](https://use-the-index-luke.com/) |

---

## 10. Key Takeaways

1. **Partition key is destiny** — it defines performance, atomicity, and cost.  
2. **Stay in single-partition ops** (point reads, transactional batches).  
3. **Use Change Feed → RabbitMQ** to publish durable, event-driven updates safely.  
4. **Monitor RU usage and 429s** like latency metrics; they’re your early-warning system.  
5. **EF Core is optional** — the SDK gives full control and visibility.  
6. **Design aggregates around access patterns, not normalization rules.**

---

**Author:** Farooq Mahmud  
**Last Updated:** November 2025
