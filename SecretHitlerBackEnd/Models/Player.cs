using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Player
    {
        public int UserId { get; set; }

        public SecretRole SecretRole { get; set; }
        public string ProfilePicture { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public dynamic Signal { get; set; }
        public bool IsDead { get; set; }
        public int RoomId { get; set; }
        public bool IsDisconnected { get; set; }
        public int Order { get; set; }

    }
}
