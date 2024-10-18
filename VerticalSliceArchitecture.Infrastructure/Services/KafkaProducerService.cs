// KafkaProducerService.cs

using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VerticalSliceArchitecture.Domain.Event;
using VerticalSliceArchitecture.Domain.Services;
using VerticalSliceArchitecture.Infrastructure.Configuration;

namespace VerticalSliceArchitecture.Infrastructure.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        
        public KafkaProducerService(IOptions<KafkaLoggingConfig> kafkaLoggingConfigOptions, ILogger<KafkaProducerService> logger)
        {
            var kafkaLoggingConfig = kafkaLoggingConfigOptions.Value;
            var config = new ProducerConfig { BootstrapServers = kafkaLoggingConfig.BootstrapServers };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
        }

        public async Task ProduceAsync(string topic, IEvent @event)
        {
            var message = new Message<string, string>
            {
                Key = @event.EventType,
                Value = JsonConvert.SerializeObject(@event)
            };

            var result = await _producer.ProduceAsync(topic, message);
            _logger.LogInformation($"Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
        }
    }
}
