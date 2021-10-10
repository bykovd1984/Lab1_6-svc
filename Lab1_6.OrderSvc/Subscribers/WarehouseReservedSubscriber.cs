using Google.Protobuf;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Billing;
using Lab1_6.Kafka.Contracts.Warehouse;
using Lab1_6.Models;
using Lab1_6.Models.Sagas;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class WarehouseReservedSubscriber : KafkaSubscriber<Reserved>
    {
        CreateOrderSaga _createOrderSaga;

        public WarehouseReservedSubscriber(ILogger<WarehouseReservedSubscriber> logger, AppConfigs config, CreateOrderSaga createOrderSaga)
            : base(logger, config)
        {
            _createOrderSaga = createOrderSaga;
        }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Warehouse_Reserved;

        public override MessageParser<Reserved> Parser => Reserved.Parser;

        protected override async Task ProcessMessage(Reserved message)
        {
            await _createOrderSaga.HandleWarehouseReserved(message.OrderId);
        }
    }
}

