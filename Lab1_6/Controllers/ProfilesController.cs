using Lab1_6.Data;
using Lab1_6.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProfilesController : Controller
    {
        UsersDbContext _usersDbContext;

        public ProfilesController(UsersDbContext usersDbContext)
        {
            _usersDbContext = usersDbContext;
        }

        [HttpPost("{userName}")]
        public async Task<ActionResult<ProfileModel>> Set(string userName, ProfileModel profileModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userName = userName.Trim();

            var profile = await _usersDbContext.Profiles.FirstOrDefaultAsync(p => p.UserName == userName);

            if(profile == null)
            {
                profile = new Profile()
                {
                    UserName = userName
                };
             
                _usersDbContext.Add(profile);
            }

            profile.Email = profileModel.Email;
            profile.Address = profileModel.Address;
            profile.Gender = profileModel.Gender;

            await _usersDbContext.SaveChangesAsync();

            return profileModel;
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<ProfileModel>> Get(string userName)
        {
            userName = userName.Trim();

            var profile = await _usersDbContext.Profiles.FirstOrDefaultAsync(u => u.UserName == userName);
            
            if (profile == null)
                profile = new Profile();

            var profileModel = new ProfileModel();

            profileModel.Address = profile.Address;
            profileModel.Gender = profile.Gender;
            profileModel.Email= profile.Email;

            return profileModel;
        }

        public class ProfileModel
        {
            public string Email { get; set; }

            public string Gender { get; set; }

            public string Address { get; set; }
        }
    }
}
