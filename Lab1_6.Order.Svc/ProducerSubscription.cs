﻿using Confluent.Kafka;
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
    public class ProducerSubscription : IHostedService
    {
        ILogger<ProducerSubscription> _logger;
        ProducerConfig _producerConfig;
        IProducer<Null, MessageModel> _producer;
        AppConfigs _config;

        public ProducerSubscription(ILogger<ProducerSubscription> logger, AppConfigs config)
        {
            _logger = logger;
            _config = config;
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
                _logger.LogDebug($"ProducerSubscription started.");
                var number = 1;
                using (_producer = new ProducerBuilder<Null, MessageModel>(_producerConfig)
                    .SetValueSerializer(new ProtoSerializer<MessageModel>()).Build())
                {
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
                                var dr = await _producer
                                    .ProduceAsync("orderCreated1", new Message<Null, MessageModel> { Value = messageModel });
                                _logger.LogDebug($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                            }
                            catch (ProduceException<Null, string> e)
                            {
                                _logger.LogError($"Delivery failed: {e.Error.Reason}");
                            }
                        }
                    }
                }

                _logger.LogDebug($"ProducerSubscription finished.");
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"ProducerSubscription stopped.");
            _producer.Dispose();
            return Task.CompletedTask;
        }
    }
}
