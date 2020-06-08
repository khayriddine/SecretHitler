using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using SecretHitlerBackEnd.Hubs;
using SecretHitlerBackEnd.InMemory;

namespace SecretHitlerBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SecretHitlerContext _context;
        private readonly IHubContext<UserHub> _userHub;
        private IMemoryCache _cache;
        
        public UsersController(SecretHitlerContext context, IHubContext<UserHub> hub,IMemoryCache cache)
        {
            _cache = cache;
            _userHub = hub;
            _context = context;
            if(_context.Users.Count() == 0)
            {
                User u1 = new User()
                {
                    Name = "khayri",
                    Email = "khayribattikh@gmail.com",
                    Password = "123",
                    Gender = Gender.Male,
                    Status = Status.Offline,
                    ImagePath = "/assets/images/unknown_male.png"
                };
                User u2 = new User()
                {
                    Name = "ghassen",
                    Email = "ghaston@gmail.com",
                    Password = "123",
                    Gender = Gender.Male,
                    Status = Status.Offline,
                    ImagePath = "/assets/images/unknown_male.png"
                };

                _context.Users.Add(u1);
                _context.Users.Add(u2);
                _context.SaveChanges();
                Friendship f1 = new Friendship()
                {
                    UserId = u1.UserId,
                    FriendId = u2.UserId,
                    Relation = RelationshipStatus.Sending
                };
                Friendship f2 = new Friendship()
                {
                    UserId = u2.UserId,
                    FriendId = u1.UserId,
                    Relation = RelationshipStatus.Pending
                };

                _context.Friendships.Add(f1);
                _context.Friendships.Add(f2);

                _context.SaveChanges();
            }



        }


        [HttpGet]
        public IActionResult Get()
        {
            List<User> users;
            if (!_cache.TryGetValue("users", out users))
            {
                users = _context.Users.ToList();
                _cache.Set("users", users);
            }
            return Ok(users);
        }



        [HttpGet("id")]
        public IActionResult GetUserById(int userId)
        {
            var user = _context.Users.Include("Friendships").First(u => u.UserId == userId);
            assignFriends(user, user.Friendships.ToList());
            return Ok(user);
        }

        [HttpPost("authenticate")]
        public  IActionResult Authenticate(UserCredentials userCredentials)
        {

            var user = _context.Users.Include("Friendships").FirstOrDefault(u =>  u.Name == userCredentials.Name  && u.Password == userCredentials.Password);
            user.Status = Status.Online;
             _context.SaveChanges();


            return Ok(assignFriends(user,user.Friendships.ToList()));
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }
        [HttpPut("{id}")]
        public IActionResult PutUser(long id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
                //var rooms = _context.Rooms.Include(r => r.UsersJoining).ToList();
                var users = _context.Users.ToList();
                _cache.Set("users", users);
                //_cache.Set("rooms", rooms);
                _userHub.Clients.All.SendAsync("Notify", "all");

            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
        [HttpGet("request")]
        public async Task<IActionResult> FriendResuest(int userId,int friendId,RequestAction choice)
        {
            var friendships  = await _context.Friendships.ToListAsync();
            switch (choice)
            {
                case RequestAction.Send:
                {
                        Friendship f1 = new Friendship()
                    {
                        UserId = userId,
                        FriendId = friendId,
                        Relation = RelationshipStatus.Sending
                    };
                    Friendship f2 = new Friendship()
                    {
                        UserId = friendId,
                        FriendId = userId,
                        Relation = RelationshipStatus.Pending
                    };
                    _context.Friendships.Add(f1);
                    _context.Friendships.Add(f2);
                break;
                }
                case RequestAction.Accept:
                {
                        var f1 = _context.Friendships.First(fs => (fs.UserId == userId && fs.FriendId == friendId));
                        var f2 = _context.Friendships.First(fs => (fs.UserId == friendId && fs.FriendId == userId));
                        f1.Relation = RelationshipStatus.Friends;
                        f2.Relation = RelationshipStatus.Friends;
                        break;
                }
                case RequestAction.Decline:
                {
                        var fss = _context.Friendships.Where(fs => ((fs.UserId == userId && fs.FriendId == friendId) || (fs.UserId == friendId && fs.FriendId == userId)));
                        _context.Friendships.RemoveRange(fss);
                        break;
                }


            }


            await _context.SaveChangesAsync();
            var users = await _context.Users.ToListAsync();
            var frds = await _context.Friendships.ToListAsync();
            User user = _context.Users.Include("Friendships").First(u => u.UserId == userId);
            assignFriends(user,user.Friendships.ToList());
            return Ok(user);
        }

        private User assignFriends(User user,List<Friendship> fships)
        {
            var friendsId = fships.Select(fs => fs.FriendId).ToArray();
            user.Friends = _context.Users.Where(u => friendsId.Contains(u.UserId)).Select<User, Friend>((u) =>
                  new Friend()
                  {
                      UserId = u.UserId,
                      Name = u.Name,
                      Email = u.Email,
                      Status = u.Status,
                      Gender = u.Gender,
                      ImagePath = u.ImagePath,
                      Relation = ((fships != null)? fships.First(fs => fs.FriendId == u.UserId).Relation : RelationshipStatus.None)
                  }
            ).ToList();
            user.Friendships.Clear();
            return user;
        }
    }
}