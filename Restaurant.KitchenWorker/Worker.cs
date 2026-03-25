using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Restaurant.Contracts;

namespace Restaurant.KitchenWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly KafkaOptions _options;

    public Worker(ILogger<Worker> logger, IOptions<KafkaOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(KafkaTopics.Orders);

        _logger.LogInformation("Kitchen worker is listening to topic {Topic}", KafkaTopics.Orders);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                var orderCreated = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);

                if (orderCreated == null)
                {
                    _logger.LogWarning("Recieved null or invalid message");
                    continue;
                }

                _logger.LogInformation("Kitchern recieved order. OrderId: {OrderId}, Table: {TableNumber}, CreatetAtUtc: {CreatedAtUtc}",
                orderCreated.OrderId,
                orderCreated.TableNumber,
                orderCreated.CreatedAtUtc);


            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming Kafka message");
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }
}
