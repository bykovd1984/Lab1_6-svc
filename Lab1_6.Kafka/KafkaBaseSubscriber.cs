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
    public static class KafkaBaseSubscriber 
    {
        public static KafkaBaseSubscriber<T> Create<T>(string groupId, string topic, MessageParser<T> parser, IServiceProvider sp)
            where T : Google.Protobuf.IMessage<T>
        {
            var logger = sp.GetRequiredService<ILogger<T>>();
            var config = sp.GetRequiredService<AppConfigs>();
            var subscriber = sp.GetRequiredService<KafkaSubscriber<T>>();

            return new KafkaBaseSubscriber<T>(groupId, topic, logger, config, subscriber, parser);
        }
    }

    public class KafkaBaseSubscriber<T> : IHostedService where T : Google.Protobuf.IMessage<T>
    {
        ILogger _logger;
        ConsumerConfig _consumerConfig;
        IConsumer<Ignore, T> _consumer;
        AppConfigs _config;
        KafkaSubscriber<T> _subscriber;
        string _topic;
        MessageParser<T> _parser;

        internal KafkaBaseSubscriber(string groupId, string topic, ILogger logger, AppConfigs config, KafkaSubscriber<T> subscriber, MessageParser<T> parser)
        {
            _logger = logger;
            _config = config;
            _topic = topic;
            _subscriber = subscriber;
            _parser = parser;
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _config.Kafka,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                _logger.LogDebug($"{typeof(T)} started. Kafka: {_config.Kafka}.");

                using (_consumer = new ConsumerBuilder<Ignore, T>(_consumerConfig)
                    .SetValueDeserializer(
                        new ProtoDeserializer<T>((bytes, isNull, context) => _parser.ParseFrom(bytes)))
                    .SetErrorHandler(
                        (consumer, error) => { _logger.LogError($"{typeof(T)} consume error: {error}"); })
                    .Build())
                {
                    _consumer.Subscribe(_topic);
                    var i = 1;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(cancellationToken);

                            _subscriber.ProcessMessage(consumeResult.Value);

                            _logger.LogDebug($"{typeof(T)}Message '{i++}' recieved: {consumeResult.Value}");
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
            _logger.LogDebug($"{typeof(T)} stopped.");
            _consumer?.Close();
            return Task.CompletedTask;
        }
    }
}
