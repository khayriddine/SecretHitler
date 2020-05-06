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
            if(_context.Users.Count() == 0)
            {
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
            var users = _context.Users.Include("Friendships").Include("Room");
            
            foreach(var user in users)
            {
                assignFriends(user,user.Friendships.ToList());
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
        public IActionResult Authenticate(UserCredentials userCredentials)
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
        public async Task<IActionResult> PutUser(int id,User user)
        {
            
            _context.Users.Include(u => u.Room).First(u => u.UserId == id).RoomId = user.RoomId;
            await _context.SaveChangesAsync();
            return Ok(_context.Users.Include(u => u.Room).First(u => u.UserId == id));
        }
        [HttpGet("request")]
        public IActionResult FriendResuest(int userId,int friendId,RequestAction choice)
        {
            var friendships = _context.Friendships;
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


            _context.SaveChanges();
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