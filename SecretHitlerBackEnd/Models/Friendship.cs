using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Friendship
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }


        public User User { get; set; }
        public User Friend { get; set; }
    }
}
