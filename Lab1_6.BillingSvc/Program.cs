using Lab1_6.BillingSvc.Subscribers;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.BillingSvc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            await host.RunAsync();
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
                        .AddEntityFrameworkNpgsql()
                        .AddDbContext<UsersDbContext>(options => options.UseNpgsql(config.UsersDB))
                        .AddScoped(typeof(KafkaProducer<>))
                        .AddHostedService<UserCreatedSubscriber>()
                        .AddHostedService<OrderRequestedSubscriber>();
                });
        }
    }
}
