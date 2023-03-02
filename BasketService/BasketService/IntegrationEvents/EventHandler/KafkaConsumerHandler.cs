using Confluent.Kafka;
using Kernal.Events;
using web.IntegrationEvents.Events;

namespace web.IntegrationEvents.EventHandler;

public class KafkaConsumerHandler:IHostedService
{
    private readonly ILogger<KafkaConsumerHandler> _logger;
    private const string Topic = "productPricechanged";
    private readonly IIntegrationEventHandler<ProductPriceChangedIntegrationEvent> _integrationEventHandler;

    public KafkaConsumerHandler(ILogger<KafkaConsumerHandler> logger, IIntegrationEventHandler<ProductPriceChangedIntegrationEvent> integrationEventHandler)
    {
        _logger = logger;
        _integrationEventHandler = integrationEventHandler;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "catlogService",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using (var builder = new ConsumerBuilder<Ignore, 
                   string>(consumerConfig).Build())
        {
            builder.Subscribe(Topic);
            var cancelToken = new CancellationTokenSource();
            try
            {
                while (true)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                    _logger.LogInformation("Message: {MessageValue} received from {ConsumerTopicPartitionOffset}", consumer.Message.Value, consumer.TopicPartitionOffset);
                    var productPriceChangedIntegrationEvent = System.Text.Json.JsonSerializer.Deserialize<ProductPriceChangedIntegrationEvent>(
                        consumer.Message.Value);
                    if (productPriceChangedIntegrationEvent != null)
                        _integrationEventHandler.Handle(productPriceChangedIntegrationEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                builder.Close();
            }
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}