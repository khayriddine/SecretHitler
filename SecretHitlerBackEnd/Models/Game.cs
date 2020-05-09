using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Game
    {
        public int GameId { get; set; }
        //public string Name { get; set; }
        //public string Password { get; set; }
        public GameStatus Status { get; set; }
        //public GameAcess GameAcess { get; set; }

        //public int BoardId { get; set; }
        //public ICollection<User> UsersPlaying { get; set; }
        //public ICollection<User> UsersWatching { get; set; }
        //public Board Board { get; set; }

        public int NumberOfRounds { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> RemainingCards { get; set; }
        public List<Card> DiscardedCards { get; set; }
        public List<Card> InHandCards { get; set; }
        public List<Card> OnTableCards { get; set; }


    }
}
