using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Player
    {
        public int UserId { get; set; }

        //public SecretRole SecretRole { get; set; }
        //public PublicRole PublicRole { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        //public int GameId { get; set; }

    }
}
