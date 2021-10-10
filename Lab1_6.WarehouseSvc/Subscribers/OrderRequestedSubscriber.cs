using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Kafka.Contracts.Warehouse;
using Lab1_6.Models;
using Lab1_6.Models.Warehouse;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1_6.WarehouseSvc.Subscribers
{
    public class OrderRequestedSubscriber : KafkaSubscriber<OrderRequested>
    {
        KafkaProducer<Reserved> _reservedKafkaProducer;
        UsersDbContext _usersDbContext;

        public OrderRequestedSubscriber(
            ILogger<OrderRequestedSubscriber> logger, 
            AppConfigs config, 
            UsersDbContext usersDbContext, 
            KafkaProducer<Reserved> reservedKafkaProducer)
            : base(logger, config)
        {
            _usersDbContext = usersDbContext;
            _reservedKafkaProducer = reservedKafkaProducer;
        }

        public override string GroupId => "WarehouseSvc";

        public override string Topic => Topics.Order_OrderRequested;

        public override MessageParser<OrderRequested> Parser => OrderRequested.Parser;

        protected override async Task ProcessMessage(OrderRequested message)
        {
            if (_usersDbContext.WarehouseReservations.Any(r => r.OrderId == message.OrderId))
                return;

            var warehouseReservation = new WarehouseReservation()
            {
                OrderId = message.OrderId,
                Status = WarehouseReservationStatus.Requested
            };

            _usersDbContext.Add(warehouseReservation);

            await _usersDbContext.SaveChangesAsync();

            await _reservedKafkaProducer.Send(Topics.Warehouse_Reserved, new Reserved() { OrderId = message.OrderId });

            _logger.LogInformation($"{typeof(OrderRequestedSubscriber)}: WarehouseReservation created for order {message.OrderId}.");
        }
    }
}

