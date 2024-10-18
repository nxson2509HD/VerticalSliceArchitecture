using Serilog.Core;
using Serilog.Events;
using Confluent.Kafka;
using Newtonsoft.Json;
using VerticalSliceArchitecture.Infrastructure.Configuration;
using VerticalSliceArchitecture.Infrastructure.Extensions;


namespace VerticalSliceArchitecture.Infrastructure.Logging
{

    public class KafkaSink : ILogEventSink
    {
        private readonly ProducerConfig _producerConfig;
        private readonly string _topic;
        private readonly IProducer<string, string> _producer;

        public KafkaSink(KafkaLoggingConfig config)
        {
            _producerConfig = new ProducerConfig { BootstrapServers = config.BootstrapServers };
            _topic = config.Topic;
            _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                var properties = new Dictionary<string, object?>(logEvent.Properties.Count);
                foreach (var property in logEvent.Properties)
                {
                    var objValue = property.Value.ExtractValue();
                    properties[property.Key] = objValue;
                }

                var logMessage = new { 
                    Message = logEvent.RenderMessage(),
                    Properties = properties
                };

                var serializedlogMessage = JsonConvert.SerializeObject(logMessage);

                var message = new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = serializedlogMessage };
                this._producer.Produce(_topic, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("KafkaSinkFail: " + ex.Message);
            }
        }

        public void Flush()
        {
            this._producer.Flush();
        }
    }
}