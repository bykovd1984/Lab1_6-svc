using Confluent.Kafka;
using Google.Protobuf;
using Lab1_6.Models;
using Lab1_6.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1_6.Kafka
{
    public abstract class KafkaSubscriber<T> : IHostedService where T : Google.Protobuf.IMessage<T>
    {
        ILogger _logger;
        ConsumerConfig _consumerConfig;
        IConsumer<Ignore, T> _consumer;
        AppConfigs _config;

        public abstract string GroupId { get; }
        public abstract string Topic { get; }
        public abstract MessageParser<T> Parser { get; }

        public KafkaSubscriber(ILogger logger, AppConfigs config)
        {
            _logger = logger;
            _config = config;
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _config.Kafka,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                GroupId = GroupId
            };
        }

        protected abstract Task ProcessMessage(T message);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            new Thread(async () =>
            {
                _logger.LogDebug($"{typeof(T)} started. Kafka: {_config.Kafka}.");

                using (_consumer = new ConsumerBuilder<Ignore, T>(_consumerConfig)
                    .SetValueDeserializer(
                        new ProtoDeserializer<T>((bytes, isNull, context) => Parser.ParseFrom(bytes)))
                    .SetErrorHandler(
                        (consumer, error) => { _logger.LogError($"{GetType()} \t consume error: {error}"); })
                    .Build())
                {
                    _consumer.Subscribe(Topic);
                    var i = 1;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(cancellationToken);

                            _logger.LogDebug($"{GetType()} \t Message recieved '{Topic}' '{GroupId}' ({consumeResult.Partition}:{consumeResult.Offset}): {consumeResult.Message.Value}");
                      
                            await ProcessMessage(consumeResult.Message.Value);

                            _logger.LogDebug($"{GetType()} \t Message processed ({consumeResult.Partition}:{consumeResult.Offset}): {consumeResult.Message.Value}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, e.Message);
                        }
                    }

                    _consumer.Close();

                    _logger.LogDebug($"{GetType()} \t ConsumerSubscription finished.");
                }
            })
            .Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
