using IdentityServerAspNetIdentity.Models;
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
    
        public UsersController(UserManager<ApplicationUser> userManager) 
        {
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<IdentityResult>> Create([FromBody]User inputUser)
        {
            var user = new ApplicationUser()
            {
                UserName = inputUser.Login
            };

            return await _userManager.CreateAsync(user, inputUser.Password);
        }

        public class User
        {
            public string Login { get; set; }

            public string Password { get; set; }
        }
    }
}
