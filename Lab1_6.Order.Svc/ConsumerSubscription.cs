using Confluent.Kafka;
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
    public class ConsumerSubscription : IHostedService
    {
        ILogger<ConsumerSubscription> _logger;

        ConsumerConfig _consumerConfig;
        IConsumer<Ignore, MessageModel> _consumer;
        AppConfigs _config;

        public ConsumerSubscription(ILogger<ConsumerSubscription> logger, AppConfigs config)
        {
            _logger = logger;
            _config = config;
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _config.Kafka,
                GroupId = "OrderSvc1",
                AutoOffsetReset = AutoOffsetReset.Earliest,

            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                _logger.LogDebug($"ConsumerSubscription started.");

                using (_consumer = new ConsumerBuilder<Ignore, MessageModel>(_consumerConfig)
                    .SetValueDeserializer(
                        new ProtoDeserializer<MessageModel>((bytes, isNull, context) => MessageModel.Parser.ParseFrom(bytes)))
                    .SetErrorHandler(
                        (consumer, error) => { _logger.LogError($"Consumer error: {error}"); })
                    .Build())
                {
                    _consumer.Subscribe("orderCreated1");
                    var i = 1;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(cancellationToken);
                            _logger.LogDebug($"Message '{i++}' recieved: {consumeResult.Value.Number}, {consumeResult.Value.Delay}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, e.Message);
                        }
                    }

                    _consumer.Close();

                    _logger.LogDebug($"ConsumerSubscription finished.");
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"ConsumerSubscription stopped.");
            _consumer.Close();
            return Task.CompletedTask; 
        }
    }
}
