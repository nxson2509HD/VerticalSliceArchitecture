using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerticalSliceArchitecture.Domain.Event;
using VerticalSliceArchitecture.Infrastructure.Configuration;

namespace VerticalSliceArchitecture.Infrastructure.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaLoggingConfig _kafkaLoggingConfig;
        private readonly string _topic = "product-events";
        private readonly string _groupId = "product-consumers";

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceProvider serviceProvider, IOptions<KafkaLoggingConfig> kafkaLoggingConfig)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _kafkaLoggingConfig = kafkaLoggingConfig.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumer(stoppingToken), stoppingToken);
        }

        private void StartConsumer(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaLoggingConfig.BootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(_topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);

                        if (consumeResult.Message != null)
                        {
                            _logger.LogInformation($"Received message: {consumeResult.Message.Value}");

                            // Parse the message as JObject
                            var jsonObject = JObject.Parse(consumeResult.Message.Value);
                            var eventType = jsonObject["EventType"]?.ToString();

                            if (string.IsNullOrEmpty(eventType))
                            {
                                _logger.LogWarning("EventType is missing in the message.");
                                continue;
                            }

                            // Xác định đầy đủ tên assembly nếu cần
                            var typeName = $"TechShopSolution.Application.Events.{eventType}, TechShopSolution.Application";
                            var eventTypeObj = Type.GetType(typeName);

                            if (eventTypeObj == null)
                            {
                                _logger.LogWarning($"Unknown event type: {eventType}");
                                continue;
                            }

                            // Deserialize vào loại sự kiện cụ thể
                            var @event = JsonConvert.DeserializeObject(consumeResult.Message.Value, eventTypeObj);

                            // Tạo một scope mới để resolve handler
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var scopedProvider = scope.ServiceProvider;

                                // Tìm kiếm handler tương ứng từ scoped provider
                                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventTypeObj);
                                var handler = scopedProvider.GetService(handlerType);

                                if (handler != null)
                                {
                                    // Gọi phương thức Handle của handler
                                    var method = handlerType.GetMethod("Handle");
                                    method.Invoke(handler, new object[] { @event, CancellationToken.None });
                                }
                                else
                                {
                                    _logger.LogWarning($"No handler found for event type: {eventType}");
                                }
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing message: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }
}
