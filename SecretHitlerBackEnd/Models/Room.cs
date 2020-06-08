using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public int NumberOfPlayer { get; set; }
        public int AdminId { get; set; }
        //public BoardSize BoardSize { get; set; }
        //public GameAcess GameAcess { get; set; }
        [NotMapped]
        public List<User> UsersJoining { get; set; }

        public void Update(Room r)
        {
            RoomId = r.RoomId;
            Name = r.Name;
            NumberOfPlayer = r.NumberOfPlayer;
            AdminId = r.AdminId;
            UsersJoining = r.UsersJoining;
        }
    }
}
