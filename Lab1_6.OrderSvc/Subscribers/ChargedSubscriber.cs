using Google.Protobuf;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Billing;
using Lab1_6.Models;
using Lab1_6.Models.Sagas;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class ChargedSubscriber : KafkaSubscriber<Charged>
    {
        CreateOrderSaga _createOrderSaga;

        public ChargedSubscriber(
            ILogger<ChargedSubscriber> logger, AppConfigs config, CreateOrderSaga createOrderSaga)
            : base(logger, config)
        {
            _createOrderSaga = createOrderSaga;
        }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Billing_Charged;

        public override MessageParser<Charged> Parser => Charged.Parser;

        protected override async Task ProcessMessage(Charged message)
        {
            await _createOrderSaga.HandleBillingCommit(message.OrderId);
        }
    }
}

