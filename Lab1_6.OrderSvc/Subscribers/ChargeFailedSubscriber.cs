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
    public class ChargeFailedSubscriber : KafkaSubscriber<ChargeFailed>
    {
        CreateOrderSaga _createOrderSaga;

        public ChargeFailedSubscriber(ILogger<ChargeFailedSubscriber> logger, AppConfigs config)
            :base (logger, config)
        { }

        public override string GroupId => "OrderSvc";

        public override string Topic => Topics.Billing_ChargeFailed;

        public override MessageParser<ChargeFailed> Parser => ChargeFailed.Parser;

        protected override async Task ProcessMessage(ChargeFailed message)
        {
            await _createOrderSaga.HandleChargeFailed(message.OrderId);
        }
    }
}

