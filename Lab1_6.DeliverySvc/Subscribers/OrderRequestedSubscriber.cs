using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Delivery;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Delivery;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1_6.DeliverySvc.Subscribers
{
    public class OrderRequestedSubscriber : KafkaSubscriber<OrderRequested>
    {
        KafkaProducer<CourierReserved> _courierReservedKafkaProducer;
        UsersDbContext _usersDbContext;

        public OrderRequestedSubscriber(
            ILogger<OrderRequestedSubscriber> logger, 
            AppConfigs config, 
            UsersDbContext usersDbContext,
            KafkaProducer<CourierReserved> courierReservedKafkaProducer)
            : base(logger, config)
        {
            _usersDbContext = usersDbContext;
            _courierReservedKafkaProducer = courierReservedKafkaProducer;
        }
            
        public override string GroupId => "DeliverySvc";

        public override string Topic => Topics.Order_OrderRequested;

        public override MessageParser<OrderRequested> Parser => OrderRequested.Parser;

        protected override async Task ProcessMessage(OrderRequested message)
        {
            if (_usersDbContext.CourierReservations.Any(r => r.OrderId == message.OrderId))
                return;

            var warehouseReservation = new CourierReservation()
            {
                OrderId = message.OrderId,
                Status = CourierReservationStatus.Requested
            };

            _usersDbContext.Add(warehouseReservation);

            await _usersDbContext.SaveChangesAsync();

            await _courierReservedKafkaProducer.Send(Topics.Delivery_CourierReserved, new CourierReserved() { OrderId = message.OrderId });

            _logger.LogInformation($"{typeof(OrderRequestedSubscriber)}: CourierReservation created for order {message.OrderId}.");
        }
    }
}

