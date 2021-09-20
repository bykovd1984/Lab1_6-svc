using Confluent.Kafka;
using Lab1_6.Kafka;
using Lab1_6.Models;
using Lab1_6.Order.Svc.Messages;
using Lab1_6.Proto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1_6.Order.Svc
{
    public class ProducerHosterService : IHostedService
    {
        ILogger<ProducerHosterService> _logger;
        ProducerConfig _producerConfig;
        KafkaProducer<MessageModel> _producer;
        AppConfigs _config;

        public ProducerHosterService(ILogger<ProducerHosterService> logger, AppConfigs config, KafkaProducer<MessageModel> producer)
        {
            _logger = logger;
            _config = config;
            _producer = producer;
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = _config.Kafka,
                SecurityProtocol = SecurityProtocol.Plaintext
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                _logger.LogDebug($"ProducerSubscription started. Kafka: {_config.Kafka}.");
                var number = 1;

                while (!cancellationToken.IsCancellationRequested)
                    {
                        var delayMs = new Random().Next(3000);

                        _logger.LogDebug($"Delay(ms): {delayMs}");

                        await Task.Delay(delayMs, cancellationToken);

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var messageModel = new MessageModel()
                            {
                                Delay = delayMs.ToString(),
                                Number = number++
                            };

                            try
                            {
                                await _producer.Send("orderCreated1", messageModel);
                            }
                            catch (ProduceException<Null, string> e)
                            {
                                _logger.LogError($"Delivery failed: {e.Error.Reason}");
                            }
                        }
                    }

                _logger.LogDebug($"ProducerSubscription finished.");
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"ProducerSubscription stopped.");
            return Task.CompletedTask;
        }
    }
}
