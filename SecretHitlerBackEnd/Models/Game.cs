using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int MyProperty { get; set; }
        public GameStatus GameStatus { get; set; }
        public GameAcess GameAcess { get; set; }

        public int BoardId { get; set; }
        public ICollection<User> UsersPlaying { get; set; }
        public ICollection<User> UsersWatching { get; set; }
        public Board Board { get; set; }

    }
}
