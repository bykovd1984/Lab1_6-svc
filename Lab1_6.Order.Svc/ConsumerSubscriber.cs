using Lab1_6.Kafka;
using Lab1_6.Order.Svc.Messages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.Order.Svc
{
    public class ConsumerSubscriber : KafkaSubscriber<MessageModel>
    {
        ILogger<ConsumerSubscriber> _logger;

        public ConsumerSubscriber(ILogger<ConsumerSubscriber> logger)
        {
            _logger = logger;
        }

        public override Task ProcessMessage(MessageModel message)
        {
            _logger.LogDebug($"Message recieved: {message.Number}, {message.Delay}");

            return Task.CompletedTask;
        }
    }
}
