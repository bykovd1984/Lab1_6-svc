using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Lab1_6.Models;
using Lab1_6.Kafka;
using Lab1_6.Order.Svc.Messages;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Proto;

namespace Lab1_6.Order.Svc
{
    class Program
    {
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureServices((_, services) =>
                {
                    var config = AppConfigs.Init(_.Configuration);

                services
                    .AddSingleton(config)
                    .AddTransient(typeof(KafkaProducer<>))
                    .AddTransient<KafkaSubscriber<MessageModel>, ConsumerSubscriber>()
                    .AddHostedService<ProducerHosterService>()
                    .AddHostedService(sp => KafkaBaseSubscriber.Create("OrderSvc", "orderCreated1", MessageModel.Parser, sp));
                        
                });
        }
    }
}
