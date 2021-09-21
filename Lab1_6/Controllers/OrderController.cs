using Lab1_6.Controllers.Models;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Kafka.Contracts.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1_6.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly KafkaProducer<CreateOrder> _kafkaProducer;

        public OrderController(KafkaProducer<CreateOrder> kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }
   
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(OrderCreateModel orderCreateModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.SelectMany(x => x.Value.Errors));

            await _kafkaProducer.Send(Topics.Order_CreateOrder, new CreateOrder() { 
                RequestId = orderCreateModel.RequestId, 
                Sum = orderCreateModel.Sum,
                UserName = User.Identity.Name
            });
            
            return Ok("Order requested.");
        }
    }
}
