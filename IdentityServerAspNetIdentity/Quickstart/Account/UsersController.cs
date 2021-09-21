using IdentityServerAspNetIdentity.Models;
using Lab1_6.Kafka;
using Lab1_6.Kafka.Contracts;
using Lab1_6.Order.Svc.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerAspNetIdentity.Quickstart.Account
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly KafkaProducer<UserCreated> _kafkaProducer;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            KafkaProducer<UserCreated> kafkaProducer) 
        {
            _userManager = userManager;
            _kafkaProducer = kafkaProducer;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<IdentityResult>> Create([FromBody]User inputUser)
        {
            var user = new ApplicationUser()
            {
                UserName = inputUser.Login
            };

            var result = await _userManager.CreateAsync(user, inputUser.Password);

            await _kafkaProducer.Send(Topics.UserCreated, new UserCreated() { UserName = user.UserName });

            return result;
        }

        public class User
        {
            public string Login { get; set; }

            public string Password { get; set; }
        }
    }
}
