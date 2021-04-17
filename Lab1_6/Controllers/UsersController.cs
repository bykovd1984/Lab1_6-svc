using Lab1_6.Data;
using Lab1_6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1_6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        UsersDbContext _usersDbContext;

        public UsersController(UsersDbContext usersDbContext)
        {
            _usersDbContext = usersDbContext;
        }

        [HttpGet("")]
        public async Task<ActionResult<User[]>> List()
        {
            var users = await _usersDbContext.Users.ToArrayAsync();

            return users;
        }


        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            user.UserName = user.UserName.Trim();

            if (await _usersDbContext.Users.AnyAsync(u => u.UserName == user.UserName))
                return BadRequest($"User with UserName='{user.UserName}' exists already.");

            _usersDbContext.Add(user);

            await _usersDbContext.SaveChangesAsync();

            return Created($"/users/{user.UserName}", user);
        }

        [HttpPost("{userName}")]
        public async Task<ActionResult<User>> Update(string userName, User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userName = userName.Trim();

            if (!await _usersDbContext.Users.AnyAsync(u => u.UserName == userName))
                return NotFound($"User with UserName='{userName}' not found.");

            if(user.UserName != userName)
                return BadRequest($"Can't change UserName.");

            _usersDbContext.Users.Update(user);

            await _usersDbContext.SaveChangesAsync();

            return user;
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<User>> Get(string userName)
        {
            userName = userName.Trim();

            var user = await _usersDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return NotFound($"User with UserName='{userName}' not found.");

            return user;
        }

        [HttpDelete("{userName}")]
        public async Task<ActionResult<User>> Delete(string userName)
        {
            userName = userName.Trim();

            var user = await _usersDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return NotFound($"User with UserName='{userName}' not found.");

            _usersDbContext.Remove(user);

            await _usersDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
