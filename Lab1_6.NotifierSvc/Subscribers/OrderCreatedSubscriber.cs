using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Notifications;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.NotifierSvc.Subscribers
{
    public class OrderCreatedSubscriber : KafkaSubscriber<OrderCreated>
    {
        ILogger<OrderCreatedSubscriber> _logger;
        UsersDbContext _usersDbContext;

        public OrderCreatedSubscriber(
            ILogger<OrderCreatedSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext)
            :base (logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
        }

        public override string GroupId => "NotifierSvc";

        public override string Topic => Topics.Order_OrderCreated;

        public override MessageParser<OrderCreated> Parser => OrderCreated.Parser;

        protected override async Task ProcessMessage(OrderCreated message)
        {
            var emailNotification = new EmailNotification()
            {
                UserName = message.UserName,
                Message = $"Order with Id='{message.OrderId} on sum '{message.Sum}' for user '{message.UserName}' created."
            };

            _usersDbContext.Add(emailNotification);

            await _usersDbContext.SaveChangesAsync();

            _logger.LogInformation($"{typeof(OrderCreatedSubscriber)}: Email for user '{emailNotification.UserName}' and message '{emailNotification.Message}' created.");
        }
    }
}

