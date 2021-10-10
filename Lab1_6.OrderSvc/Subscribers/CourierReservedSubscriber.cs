using Google.Protobuf;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Billing;
using Lab1_6.Kafka.Contracts.Delivery;
using Lab1_6.Kafka.Contracts.Warehouse;
using Lab1_6.Models;
using Lab1_6.Models.Sagas;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class CourierReservedSubscriber : KafkaSubscriber<CourierReserved>
    {
        CreateOrderSaga _createOrderSaga;

        public CourierReservedSubscriber(ILogger<WarehouseReservedSubscriber> logger, AppConfigs config, CreateOrderSaga createOrderSaga)
            :base (logger, config)
        {
            _createOrderSaga = createOrderSaga;
        }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Delivery_CourierReserved;

        public override MessageParser<CourierReserved> Parser => CourierReserved.Parser;

        protected override async Task ProcessMessage(CourierReserved message)
        {
            await _createOrderSaga.HandleCourierReserved(message.OrderId);
        }
    }
}

