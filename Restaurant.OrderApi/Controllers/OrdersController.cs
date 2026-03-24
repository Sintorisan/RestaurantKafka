using System.Formats.Asn1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Contracts;

namespace Restaurant.OrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderEventProducer _producer;

    public OrdersController(IOrderEventProducer producer)
    {
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var @event = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid().ToString(),
            TableNumber = request.TableNumber,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _producer.PublishOrderCreatedAsync(@event, cancellationToken);

        return Accepted(new
        {
            message = "order.creates published",
            orderIs = @event.OrderId
        });
    }
}

