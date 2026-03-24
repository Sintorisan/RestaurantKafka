using Restaurant.Contracts;

namespace Restaurant.OrderApi;

public interface IOrderEventProducer
{
    Task PublishOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default);
}
