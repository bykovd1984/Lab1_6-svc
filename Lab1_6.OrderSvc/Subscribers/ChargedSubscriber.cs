using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Billing;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class ChargedSubscriber : KafkaSubscriber<Charged>
    {
        KafkaProducer<OrderCretated> _orderCretatedKafkaProducer;
        ILogger<ChargedSubscriber> _logger;
        UsersDbContext _usersDbContext;

        public ChargedSubscriber(
            ILogger<ChargedSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext,
            KafkaProducer<OrderCretated> orderCretatedKafkaProducer)
            :base (logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
            _orderCretatedKafkaProducer = orderCretatedKafkaProducer;
        }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Billing_Charged;

        public override MessageParser<Charged> Parser => Charged.Parser;

        protected override async Task ProcessMessage(Charged message)
        {
            var order = await _usersDbContext.Orders
                .FirstOrDefaultAsync(r => r.Id == message.OrderId);

            if (order == null)
            {
                _logger.LogError($"{GetType()} Order with Id='{message.OrderId}' not found.");
                return;
            }

            if(order.Status != OrderStatus.Creating)
            {
                _logger.LogError($"{GetType()} Order with Id='{message.OrderId}' has status '{order.Status}' and can't change to '{OrderStatus.Created}'.");
                return;
            }

            order.Status = OrderStatus.Created;

            await _usersDbContext.SaveChangesAsync();

            await _orderCretatedKafkaProducer.Send(Topics.Order_OrderCreated, new OrderCretated()
            {
                UserName = order.UserName,
                Sum = order.Sum,
                OrderId = order.Id
            });

            _logger.LogInformation($"{typeof(ChargedSubscriber)} Order with Id='{order.Id}' change status to '{OrderStatus.Created}'.");
        }
    }
}

