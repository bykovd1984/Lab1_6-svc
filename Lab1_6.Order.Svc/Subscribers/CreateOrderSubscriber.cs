using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models;
using Lab1_6.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.OrderSvc.Subscribers
{
    public class CreateOrderSubscriber : KafkaSubscriber<CreateOrder>
    {
        ILogger<CreateOrderSubscriber> _logger;
        UsersDbContext _usersDbContext;

        public CreateOrderSubscriber(
            ILogger<CreateOrderSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext)
            :base (logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
        }

        public override string GroupId => "BillingSvc";

        public override string Topic => Topics.Order_CreateOrder;

        public override MessageParser<CreateOrder> Parser => CreateOrder.Parser;

        protected override async Task ProcessMessage(CreateOrder message)
        {
            _logger.LogDebug($"{typeof(CreateOrderSubscriber)} Message recieved: UserName='{message.UserName}'.");

            var existingOrder = await _usersDbContext.OrderRequests
                .FirstOrDefaultAsync(r => r.RequestId == message.RequestId);

            if (existingOrder != null)
            {
                _logger.LogInformation($"{typeof(CreateOrderSubscriber)} Message with RequestId='{message.RequestId}' for UserName='{message.UserName}'  has been processed already and this message is ignored.");
                return;
            }

            var order = new Order()
            {
                Status = OrderStatus.Creating,
                Sum = message.Sum,
                UserName = message.UserName
            };

            var orderRequest = new OrderRequest()
            {
                Order = order,
                RequestId = message.RequestId
            };

            _usersDbContext.Add(orderRequest);
            
            await _usersDbContext.SaveChangesAsync();

            _logger.LogInformation($"{typeof(CreateOrderSubscriber)} Order with Id='{order.Id}' for UserName='{message.UserName}' created.");
        }
    }
}

