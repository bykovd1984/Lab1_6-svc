using Confluent.Kafka;
using Lab1_6.Models;
using Lab1_6.Proto;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.Kafka
{
    public class KafkaProducer<T> where T : Google.Protobuf.IMessage<T>
    {
        ILogger<KafkaProducer<T>> _logger;
        ProducerConfig _producerConfig;
        IProducer<Null, T> _producer;
        AppConfigs _config;

        public KafkaProducer(ILogger<KafkaProducer<T>> logger, AppConfigs config)
        {
            _logger = logger;
            _config = config;
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = _config.Kafka,
                SecurityProtocol = SecurityProtocol.Plaintext
            };
        }

        public async Task Send(string topic, T message)
        {
            using (_producer = new ProducerBuilder<Null, T>(_producerConfig)
                .SetValueSerializer(new ProtoSerializer<T>()).Build())
            {
                try
                {
                    var dr = await _producer
                        .ProduceAsync(topic, new Message<Null, T> { Value = message });

                    _logger.LogDebug($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}' for '{topic}'.");
                }
                catch (ProduceException<Null, string> e)
                {
                    _logger.LogError($"Delivery failed: {e.Error.Reason}");
                    throw;
                }
            }
        }
    }
}
