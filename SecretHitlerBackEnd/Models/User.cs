using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public Gender Gender { get; set; }
        public Status Status { get; set; }
        public int? RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
        [NotMapped]
        public virtual ICollection<Friend> Friends { get; set; }
        public ICollection<Friendship> Friendships { get; set; }

        //public int PlayerId { get; set; }
        //public int StatisticsId { get; set; }
        //public int RoomId { get; set; }
        //
        //public Player Player { get; set; }
        //public Statistics Statistics { get; set; }

    }

    public class UserCredentials
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
