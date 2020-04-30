using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Player
    {
        public int PlayerId { get; set; }

        public SecretRole SecretRole { get; set; }
        public PublicRole PublicRole { get; set; }

        public int UserId { get; set; }
        public int GameId { get; set; }
        public User User { get; set; }
        public Game Game { get; set; }

    }
}
