using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Billing;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lab1_6.BillingSvc.Subscribers
{
    public class OrderChargeRequestedSubscriber : KafkaSubscriber<OrderChargeRequested>
    {
        ILogger<OrderChargeRequestedSubscriber> _logger;
        UsersDbContext _usersDbContext;
        KafkaProducer<Charged> _chargedKafkaProducer;
        KafkaProducer<ChargeFailed> _chargeFailedKafkaProducer;

        public OrderChargeRequestedSubscriber(
            ILogger<OrderChargeRequestedSubscriber> logger, 
            AppConfigs config, 
            UsersDbContext usersDbContext, 
            KafkaProducer<Charged> chargedKafkaProducer, 
            KafkaProducer<ChargeFailed> chargeFailedKafkaProducer)
            :base (logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
            _chargedKafkaProducer = chargedKafkaProducer;
            _chargeFailedKafkaProducer = chargeFailedKafkaProducer;
        }

        public override string GroupId => "BillingSvc";

        public override string Topic => Topics.Order_OrderChargeRequested;

        public override MessageParser<OrderChargeRequested> Parser => OrderChargeRequested.Parser;

        protected override async Task ProcessMessage(OrderChargeRequested message)
        {
            _logger.LogDebug($"{typeof(OrderChargeRequestedSubscriber)} Message recieved: UserName='{message.UserName}', Sum='{message.Sum}'.");

            var account = await _usersDbContext.Accounts.FirstOrDefaultAsync(a => a.UserName == message.UserName);

            if (account == null)
                throw new Exception($"Account for '{message.UserName}' not found.");

            if (account.Deposit < message.Sum)
            {
                await _chargeFailedKafkaProducer.Send(Topics.Billing_ChargeFailed, new ChargeFailed()
                {
                    OrderId = message.OrderId,
                    RequestedSum = message.Sum,
                    CurrentSum = account.Deposit,
                });

                _logger.LogInformation($"{typeof(OrderChargeRequestedSubscriber)} Sum '{message.Sum}' for UserName='{message.UserName}' can't charge. Current value is '{account.Deposit}'.");

                return;
            }

            account.Deposit = account.Deposit - message.Sum;

            await _usersDbContext.SaveChangesAsync();

            await _chargedKafkaProducer.Send(Topics.Billing_Charged, new Charged()
            {
                OrderId = message.OrderId,
                NewSum = account.Deposit,
            });

            _logger.LogInformation($"{typeof(OrderChargeRequestedSubscriber)} Sum '{message.Sum}' for UserName='{message.UserName}' has charged. New value is '{account.Deposit}'.");
        }
    }
}

