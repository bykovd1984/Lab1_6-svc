using Google.Protobuf;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Delivery;
using Lab1_6.Models;
using Lab1_6.Models.Sagas;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class CourierReservationFailedSubscriber : KafkaSubscriber<CourierReservationFailed>
    {
        CreateOrderSaga _createOrderSaga;

        public CourierReservationFailedSubscriber(ILogger<CourierReservationFailedSubscriber> logger, AppConfigs config, CreateOrderSaga createOrderSaga)
            : base(logger, config)
        {
            _createOrderSaga = createOrderSaga;
        }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Delivery_CourierReservationFailed;

        public override MessageParser<CourierReservationFailed> Parser => CourierReservationFailed.Parser;

        protected override async Task ProcessMessage(CourierReservationFailed message)
        {
            await _createOrderSaga.HandleCourierReservationFailed(message.OrderId);
        }
    }
}

