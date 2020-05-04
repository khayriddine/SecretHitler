using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Friend
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public Gender Gender { get; set; }
        public Status Status { get; set; }
        public RelationshipStatus Relation { get; set; }
    }
}
