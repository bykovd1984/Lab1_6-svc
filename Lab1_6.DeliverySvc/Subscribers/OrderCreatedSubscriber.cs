using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Warehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.DeliverySvc.Subscribers
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

        public override string GroupId => "DeliverySvc";

        public override string Topic => Topics.Order_OrderCreated;

        public override MessageParser<OrderCreated> Parser => OrderCreated.Parser;

        protected override async Task ProcessMessage(OrderCreated message)
        {
            var warehouseReservation = await _usersDbContext.WarehouseReservations
                .FirstOrDefaultAsync(r => r.OrderId == message.OrderId);

            if(warehouseReservation.Status == WarehouseReservationStatus.Requested)
            {
                warehouseReservation.Status = WarehouseReservationStatus.Commited;
                await _usersDbContext.SaveChangesAsync();
            }    

            _logger.LogInformation($"{typeof(OrderRequestedSubscriber)}: WarehouseReservation commited for order {message.OrderId}.");
        }
    }
}

