using Lab1_6.Data;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Lab1_6.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lab1_6.Models.Sagas
{
    public class CreateOrderSaga
    {
        public static object _lock = new object();

        KafkaProducer<OrderRequested> _orderRequestedKafkaProducer;
        KafkaProducer<OrderChargeRequested> _orderChargeRequestedKafkaProducer;
        KafkaProducer<OrderCreated> _orderCreatedKafkaProducer;
        KafkaProducer<OrderFailed> _orderFailedKafkaProducer;
        ILogger<CreateOrderSaga> _logger;
        UsersDbContext _usersDbContext;

        public CreateOrderSaga(KafkaProducer<OrderRequested> orderRequestedKafkaProducer,
            KafkaProducer<OrderChargeRequested> orderChargeRequestedKafkaProducer,
            KafkaProducer<OrderCreated> orderCreatedKafkaProducer,
            KafkaProducer<OrderFailed> orderFailedKafkaProducer,
            ILogger<CreateOrderSaga> logger,
            UsersDbContext usersDbContext)
        {
            _orderRequestedKafkaProducer = orderRequestedKafkaProducer;
            _orderChargeRequestedKafkaProducer = orderChargeRequestedKafkaProducer;
            _orderCreatedKafkaProducer = orderCreatedKafkaProducer;
            _orderFailedKafkaProducer = orderFailedKafkaProducer;
            _logger = logger;
            _usersDbContext = usersDbContext;
        }

        public async Task CreateOrder(CreateOrder message)
        {
            var existingOrder = await _usersDbContext.OrderRequests
                .FirstOrDefaultAsync(r => r.RequestId == message.RequestId);

            if (existingOrder != null)
            {
                _logger.LogInformation($"{typeof(CreateOrderSaga)} Request with RequestId='{message.RequestId}' for UserName='{message.UserName}'  has been processed already and this message is ignored.");
                return;
            }

            var order = new Order()
            {
                Status = OrderStatus.PendingWarehouseAndDeliveryCommit,
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

            await _orderRequestedKafkaProducer.Send(Topics.Order_OrderRequested, new OrderRequested()
            {
                OrderId = order.Id,
                UserName = order.UserName,
                Sum = order.Sum
            });

            _logger.LogInformation($"{typeof(CreateOrderSaga)} Order with Id='{order.Id}' for UserName='{message.UserName}' created.");
        }

        public async Task HandleWarehouseReserved(int orderId)
        {
            lock (_lock)
            {
                var order = _usersDbContext.Orders.Find(orderId);

                switch (order.Status)
                {
                    case OrderStatus.PendingWarehouseAndDeliveryCommit:
                        order.Status = OrderStatus.PendingDeliveryCommit;
                        _usersDbContext.SaveChanges();
                        break;
                    case OrderStatus.PendingWarehouseCommit:
                        order.Status = OrderStatus.PendingBillingCommit;
                        _usersDbContext.SaveChanges();
                        SendBillingRequest(order).Wait();
                        break;
                    default:
                        return;
                }

            }
        }

        public async Task HandleDeliveryCommit(int orderId)
        {
            lock (_lock)
            {
                var order = _usersDbContext.Orders.Find(orderId);

                switch (order.Status)
                {
                    case OrderStatus.PendingWarehouseAndDeliveryCommit:
                        order.Status = OrderStatus.PendingWarehouseCommit;
                        _usersDbContext.SaveChanges();
                        break;
                    case OrderStatus.PendingDeliveryCommit:
                        order.Status = OrderStatus.PendingBillingCommit;
                        _usersDbContext.SaveChanges();
                        SendBillingRequest(order).Wait();
                        break;
                    default:
                        return;
                }
            }
        }

        public async Task HandleBillingCommit(int orderId)
        {
            lock (_lock)
            {
                var order = _usersDbContext.Orders.Find(orderId);

                switch (order.Status)
                {
                    case OrderStatus.PendingBillingCommit:
                        order.Status = OrderStatus.Created;
                        _usersDbContext.SaveChanges();
                        SendOrderCreated(order).Wait();
                        break;
                    default:
                        return;
                }
            }
        }

        public async Task HandleChargeFailed(int orderId)
        {
            lock (_lock)
            {
                var order = _usersDbContext.Orders.Find(orderId);

                switch (order.Status)
                {
                    case OrderStatus.PendingBillingCommit:
                        order.Status = OrderStatus.Cancelled;
                        _usersDbContext.SaveChanges();
                        SendOrderFailedAsync(order).Wait();
                        break;
                    default:
                        return;
                }
            }
        }

        public async Task HandleWarehouseReservationFailed(int orderId)
        {
            lock (_lock)
            {
                var order = _usersDbContext.Orders.Find(orderId);

                switch (order.Status)
                {
                    case OrderStatus.PendingWarehouseAndDeliveryCommit:
                    case OrderStatus.PendingWarehouseCommit:
                        order.Status = OrderStatus.Cancelled;
                        _usersDbContext.SaveChanges();
                        SendOrderFailedAsync(order).Wait();
                        break;
                    default:
                        return;
                }
            }
        }

        private async Task SendOrderFailedAsync(Order order)
        {
            await _orderFailedKafkaProducer.Send(Topics.Order_OrderFailed, new OrderFailed()
            {
                OrderId = order.Id,
                Sum = order.Sum,
                UserName = order.UserName
            });
        }

        private async Task SendOrderCreated(Order order)
        {
            await _orderCreatedKafkaProducer.Send(Topics.Order_OrderCreated, new OrderCreated()
            {
                OrderId = order.Id,
                Sum = order.Sum,
                UserName = order.UserName
            });
        }

        private async Task SendBillingRequest(Order order)
        {
            await _orderChargeRequestedKafkaProducer.Send(Topics.Order_OrderChargeRequested, new OrderChargeRequested()
            {
                OrderId = order.Id,
                Sum = order.Sum,
                UserName = order.UserName
            });
        }
    }
}
