using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Notifications;
using Lab1_6.Models.Warehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.DeliverySvc.Subscribers
{
    public class OrderFailedSubscriber : KafkaSubscriber<OrderFailed>
    {
        UsersDbContext _usersDbContext;

        public OrderFailedSubscriber(
            ILogger<OrderFailedSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext)
            : base(logger, config)
        {
            _usersDbContext = usersDbContext;
        }

        public override string GroupId => "DeliverySvc";

        public override string Topic => Topics.Order_OrderFailed;

        public override MessageParser<OrderFailed> Parser => OrderFailed.Parser;

        protected override async Task ProcessMessage(OrderFailed message)
        {
            var warehouseReservation = await _usersDbContext.WarehouseReservations
                .FirstOrDefaultAsync(r => r.OrderId == message.OrderId);

            if (warehouseReservation.Status == WarehouseReservationStatus.Requested)
            {
                warehouseReservation.Status = WarehouseReservationStatus.Cancelled;
                await _usersDbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"{typeof(OrderRequestedSubscriber)}: WarehouseReservation cancelled for order {message.OrderId}.");
        }
    }
}

