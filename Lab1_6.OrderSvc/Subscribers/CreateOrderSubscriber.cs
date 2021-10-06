using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Sagas;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class CreateOrderSubscriber : KafkaSubscriber<CreateOrder>
    {
        CreateOrderSaga _createOrderSaga;
        ILogger<CreateOrderSubscriber> _logger;

        public CreateOrderSubscriber(
            ILogger<CreateOrderSubscriber> logger, AppConfigs config, CreateOrderSaga createOrderSaga)
            :base (logger, config)
        {
            _logger = logger;
            _createOrderSaga = createOrderSaga;
        }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Order_CreateOrder;

        public override MessageParser<CreateOrder> Parser => CreateOrder.Parser;

        protected override async Task ProcessMessage(CreateOrder message)
        {
            _logger.LogDebug($"{typeof(CreateOrderSubscriber)} Message recieved: UserName='{message.UserName}'.");

            await _createOrderSaga.CreateOrder(message);
        }
    }
}

