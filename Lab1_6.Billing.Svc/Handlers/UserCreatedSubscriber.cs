using Google.Protobuf;
using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Models;
using Lab1_6.Models.Billing;
using Lab1_6.Order.Svc.Messages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lab1_6.Billing.Svc.Handlers
{
    public class UserCreatedSubscriber : KafkaSubscriber<UserCreated>
    {
        ILogger<UserCreatedSubscriber> _logger;
        UsersDbContext _usersDbContext;

        public UserCreatedSubscriber(
            ILogger<UserCreatedSubscriber> logger, AppConfigs config, UsersDbContext usersDbContext)
            :base (logger, config)
        {
            _logger = logger;
            _usersDbContext = usersDbContext;
        }

        public override string GroupId => "BillingSvc";

        public override string Topic => Topics.UserCreated;

        public override MessageParser<UserCreated> Parser => UserCreated.Parser;

        protected override async Task ProcessMessage(UserCreated message)
        {
            _logger.LogDebug($"{typeof(UserCreatedSubscriber)} Message recieved: UserName='{message.UserName}'.");

            var account = new Account()
            {
                UserName = message.UserName,
                Deposit = 100
            };

            _usersDbContext.Add(account);
            
            await _usersDbContext.SaveChangesAsync();

            _logger.LogInformation($"{typeof(UserCreatedSubscriber)} Account for UserName='{message.UserName}' created.");
        }
    }
}

