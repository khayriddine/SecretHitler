using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using BackEnd.Repositories;

namespace SecretHitlerBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly SecretHitlerContext _context;

        public RoomsController(SecretHitlerContext context)
        {
            _context = context;
            /*
            var user = _context.Users.Find(1);
            var room = _context.Rooms.Find(1);
            room.UsersJoining = new List<User>();
            room.UsersJoining.Add(user);
            _context.SaveChanges();*/
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.Include(r => r.UsersJoining ).ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.RoomId)
            {
                return BadRequest();
            }

            var r = _context.Rooms.Find(id);
            r.RoomId = r.RoomId;
            r.Name = r.Name;
            r.NumberOfPlayer = r.NumberOfPlayer;
            r.AdminId = r.AdminId;
            foreach(var user in room.UsersJoining)
            {
                _context.Users.Find(user.UserId).Room = r;
            }
            try
            {
                await _context.SaveChangesAsync();
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
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.RoomId }, room);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Room>> DeleteRoom(int id)
        {
            var room = await _context.Rooms.Include(r => r.UsersJoining).FirstAsync(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            foreach(var user in room.UsersJoining)
            {
                user.RoomId = null;
            }
            await _context.SaveChangesAsync();
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return room;
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Include("UsersJoining").Any(e => e.RoomId == id);
        }
    }
}
