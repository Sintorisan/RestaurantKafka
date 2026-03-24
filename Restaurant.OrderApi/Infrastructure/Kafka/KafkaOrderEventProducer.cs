using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Restaurant.Contracts;

namespace Restaurant.OrderApi;

public sealed class KafkaOrderEventProducer : IOrderEventProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaOrderEventProducer(IOptions<KafkaOptions> options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }


    public async Task PublishOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(message);

        var kafkaMessage = new Message<string, string>
        {
            Key = message.OrderId,
            Value = payload
        };

        await _producer.ProduceAsync(KafkaTopics.Orders, kafkaMessage, cancellationToken);
    }
}
