using Lab1_6.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Lab1_6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailsController : Controller
    {
        private readonly UsersDbContext _usersDbContext;

        public EmailsController(UsersDbContext usersDbContext)
        {
            _usersDbContext = usersDbContext;
        }
   
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var emails = await _usersDbContext.EmailNotifications.ToListAsync();
            
            return Ok(emails);
        }
    }
}
