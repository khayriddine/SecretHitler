using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Board
    {
        public int BoardId { get; set; }
        public string ImagePath { get; set; }
        public BoardSize BoardSize { get; set; }
    }
}
