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
    public class ChargeFailedSubscriber : KafkaSubscriber<ChargeFailed>
    {
        KafkaProducer<OrderFailed> _orderFailedKafkaProducer;
        ILogger<ChargeFailedSubscriber> _logger;
        UsersDbContext _usersDbContext;

        public ChargeFailedSubscriber(
            ILogger<ChargeFailedSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext,
            KafkaProducer<OrderFailed> orderFailedKafkaProducer)
            :base (logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
            _orderFailedKafkaProducer = orderFailedKafkaProducer;
        }

        public override string GroupId => "BillingSvc";

        public override string Topic => Topics.Billing_ChargeFailed;

        public override MessageParser<ChargeFailed> Parser => ChargeFailed.Parser;

        protected override async Task ProcessMessage(ChargeFailed message)
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

            order.Status = OrderStatus.Cancelled;

            await _usersDbContext.SaveChangesAsync();

            await _orderFailedKafkaProducer.Send(Topics.Order_OrderCreated, new OrderFailed()
            {
                UserName = order.UserName,
                Sum = message.RequestedSum,
                OrderId = order.Id,
                Reason = $"You don't have enough money on your account for order. You have '{message.CurrentSum}', order sum '{message.RequestedSum}'."
            });

            _logger.LogInformation($"{typeof(ChargeFailedSubscriber)} Order with Id='{order.Id}' change status to '{OrderStatus.Cancelled}'.");
        }
    }
}

