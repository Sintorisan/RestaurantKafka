namespace Restaurant.Contracts;

public sealed class OrderCreatedEvent
{
    public string EventType { get; init; } = "order.create";
    public string OrderId { get; init; } = string.Empty;
    public int TableNumber { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
