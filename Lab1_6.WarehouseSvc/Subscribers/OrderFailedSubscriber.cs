using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Notifications;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.WarehouseSvc.Subscribers
{
    public class OrderFailedSubscriber : KafkaSubscriber<OrderFailed>
    {
        KafkaProducer<OrderFailed> _chargedKafkaProducer;
        ILogger<OrderFailedSubscriber> _logger;
        UsersDbContext _usersDbContext;

        public OrderFailedSubscriber(
            ILogger<OrderFailedSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext,
            KafkaProducer<OrderFailed> chargedKafkaProducer)
            : base(logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
            _chargedKafkaProducer = chargedKafkaProducer;
        }

        public override string GroupId => "NotifierSvc";

        public override string Topic => Topics.Order_OrderFailed;

        public override MessageParser<OrderFailed> Parser => OrderFailed.Parser;

        protected override async Task ProcessMessage(OrderFailed message)
        {
            var emailNotification = new EmailNotification()
            {
                UserName = message.UserName,
                Message = $"Order with Id='{message.OrderId} on sum '{message.Sum}' for user '{message.UserName}' failed."
            };

            _usersDbContext.Add(emailNotification);

            await _usersDbContext.SaveChangesAsync();

            _logger.LogInformation($"{typeof(OrderCreatedSubscriber)}: Email for user '{emailNotification.UserName}' and message '{emailNotification.Message}' failed.");
        }
    }
}

