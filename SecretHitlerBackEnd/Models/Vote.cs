using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Vote
    {
        public int Value { get; set; } //0 = no 1 = yes
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
