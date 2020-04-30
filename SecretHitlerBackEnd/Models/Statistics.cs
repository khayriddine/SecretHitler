using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Statistics
    {
        public int StatisticId { get; set; }
        public int NumberOfPlayedGames { get; set; }
        public int NumberOfWiningGames { get; set; }
        public int UserId { get; set; }


        public User User { get; set; }
    }
}
