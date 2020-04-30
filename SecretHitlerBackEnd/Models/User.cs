using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public Gender Gender { get; set; }
        public Status Status { get; set; }

        public virtual ICollection<Friendship> UserFriends { get; set; }
        public virtual ICollection<Friendship> Friends { get; set; }
        //public int PlayerId { get; set; }
        //public int StatisticsId { get; set; }
        //public int RoomId { get; set; }
        //
        //public Player Player { get; set; }
        //public Statistics Statistics { get; set; }

    }
}
