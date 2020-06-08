using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR;
using SecretHitlerBackEnd.Hubs;

namespace SecretHitlerBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly SecretHitlerContext _context;
        private readonly IHubContext<UserHub> _userHub;
        private IMemoryCache _cache;
        public RoomsController(SecretHitlerContext context, IHubContext<UserHub> Hub, IMemoryCache cache)
        {
            _userHub = Hub;
            _context = context;
            _cache = cache;
            /*
            var user = _context.Users.Find(1);
            var room = _context.Rooms.Find(1);
            room.UsersJoining = new List<User>();
            room.UsersJoining.Add(user);
            _context.SaveChanges();*/
        }

        // GET: api/Rooms
        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            var rooms = getAllRooms();
            return Ok(rooms);
        }
        private List<Room> getAllRooms()
        {
            List<Room> rooms;
            lock (_cache)
            {
                
                if (!_cache.TryGetValue("rooms", out rooms))
                {
                    rooms = _context.Rooms.ToList();
                    _cache.Set("rooms", rooms);
                }
                
            }
            return rooms;
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(int id)
        {
            var rooms = getAllRooms();
            var room = rooms.Find(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            return room;
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public IActionResult PutRoom(int id, Room room)
        {
            if (id != room.RoomId)
            {
                return BadRequest();
            }

            var r =  _context.Rooms.Find(id);
            r.RoomId = r.RoomId;
            r.Name = r.Name;
            r.NumberOfPlayer = r.NumberOfPlayer;
            r.AdminId = r.AdminId;
            try
            {
                 _context.SaveChanges();
                var rooms = _context.Rooms.Include(ro => ro.UsersJoining).ToList();
                _cache.Set("rooms", rooms);
                _userHub.Clients.All.SendAsync("Notify", "rooms");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(room);
        }

        // POST: api/Rooms
        [HttpPost]
        public ActionResult<Room> PostRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            var rooms = _context.Rooms.Include(r => r.UsersJoining).ToList();
            _cache.Set("rooms", rooms);
            _userHub.Clients.All.SendAsync("Notify", "rooms");
            return CreatedAtAction("GetRoom", new { id = room.RoomId }, room);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public ActionResult<Room> DeleteRoom(int id)
        {
            var room = _context.Rooms.Include(r => r.UsersJoining).First(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            foreach(var user in room.UsersJoining)
            {
                user.RoomId = null;
            }
             _context.SaveChanges();
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            var rooms = _context.Rooms.Include(r => r.UsersJoining).ToList();
            _cache.Set("rooms", rooms);
            _userHub.Clients.All.SendAsync("Notify", "rooms");
            return room;
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Include("UsersJoining").Any(e => e.RoomId == id);
        }
    }
}
