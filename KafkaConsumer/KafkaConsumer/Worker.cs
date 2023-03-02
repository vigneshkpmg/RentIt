using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using Confluent.Kafka;
using OpenTelemetry.Trace;
using Serilog.Context;

namespace KafkaConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private const string Topic = "productPricechanged";
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly ActivitySource KafkaConsumerActivitySource = new("KafkaConsumer","1.0.0");
    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async  Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "catalogService",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var builder = new ConsumerBuilder<Ignore, 
            string>(consumerConfig).Build();
        builder.Subscribe(Topic);
        var cancelToken = new CancellationTokenSource();

           
            while (!stoppingToken.IsCancellationRequested)
            {
                
                try
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    var parentId = System.Text.Encoding.UTF8.GetString(consumer.Message.Headers[0].GetValueBytes());
                    using var parentActivity = KafkaConsumerActivitySource.StartActivity("Consuming Message", ActivityKind.Consumer, parentId);
                    Console.WriteLine(
                        $"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");

                    _logger.LogInformation("Message: {MessageValue} received from {ConsumerTopicPartitionOffset}",
                        consumer.Message.Value, consumer.TopicPartitionOffset);
                    
                    if (string.IsNullOrEmpty(consumer.Message.Value)) 
                        continue;
                    
                    var content = new StringContent(consumer.Message.Value, Encoding.UTF8,
                        MediaTypeNames.Application.Json);
                    var client = _httpClientFactory.CreateClient();
                    using var childActivity =
                        KafkaConsumerActivitySource.StartActivity("Basket-API", ActivityKind.Client);
                    await client.PatchAsync(new Uri("https://localhost:7186/api/v1/Basket"), content,
                        cancelToken.Token);
                    _logger.LogInformation("price updated");
                }

                catch (Exception ex)
                {
   
                    _logger.LogError("Error occured with message :{ExMessage} and error: {ExInnerException}",
                        ex.Message, ex.InnerException);
                    builder.Close();
                }
            }

    }
}