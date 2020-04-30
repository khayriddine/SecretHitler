using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecretHitlerBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SecretHitlerContext _context;
        public UsersController(SecretHitlerContext context)
        {
            _context = context;

            User u1 = new User() {
                Name = "khayri",
                Email ="khayribattikh@gmail.com",
                Password ="123",
                ImagePath ="none",
                Gender = Gender.Male,
                Status = Status.Offline
            };
            User u2 = new User()
            {
                Name = "ghassen",
                Email = "ghaston@gmail.com",
                Password = "123",
                ImagePath = "none",
                Gender = Gender.Male,
                Status = Status.Offline
            };
            _context.Users.Add(u1);
            _context.Users.Add(u2);
            Friendship f1 = new Friendship()
            {
                UserId = u1.ID,
                FriendId = u2.ID
            };
            
            
            _context.Friendships.Add(f1);
            _context.SaveChangesAsync();
        }


        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return _context.Users;
        }
    }
}