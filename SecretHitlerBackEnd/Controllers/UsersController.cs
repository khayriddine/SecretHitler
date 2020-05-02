using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
            /*
            User u1 = new User()
            {
                Name = "khayri",
                Email = "khayribattikh@gmail.com",
                Password = "123",
                Gender = Gender.Male,
                Status = Status.Offline,
                ImagePath = "none"
            };
            User u2 = new User()
            {
                Name = "ghassen",
                Email = "ghaston@gmail.com",
                Password = "123",
                Gender = Gender.Male,
                Status = Status.Offline,
                ImagePath = "none"
            };

            _context.Users.Add(u1);
            _context.Users.Add(u2);
            _context.SaveChanges();
            Friendship f1 = new Friendship()
            {
                UserId = u1.UserId,
                FriendId = u2.UserId
            };
            Friendship f2 = new Friendship()
            {
                UserId = u2.UserId,
                FriendId = u1.UserId
            };

            _context.Friendships.Add(f1);
            _context.Friendships.Add(f2);
            _context.SaveChanges();
            */
        }


        [HttpGet]
        public IActionResult Get()
        {
            var users = _context.Users.Include("Friendships");
            
            foreach(var user in users)
            {
                var friendsId = user.Friendships.Select(fs => fs.FriendId).ToArray();
                user.Friends = _context.Users.Where(u => friendsId.Contains(u.UserId)).ToList();
                user.Friendships.Clear();
            }
            return Ok(users);
        }
        
        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserCredentials userCredentials)
        {

            var user = _context.Users.Include("Friendships").FirstOrDefault(u =>  u.Name == userCredentials.Name  && u.Password == userCredentials.Password);
            user.Status = Status.Online;
            _context.SaveChanges();
            var friendsId = user.Friendships.Select(fs => fs.FriendId).ToArray();
            user.Friends = _context.Users.Where(u => friendsId.Contains(u.UserId)).ToList();
            user.Friendships.Clear();
            
            return Ok(user);
        }
    }
}