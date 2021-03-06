﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Friendship
    {
        public int FriendshipId { get; set; }
        public int FriendId { get; set; }
        public int UserId { get; set; }
        public RelationshipStatus Relation { get; set; }
        public User User { get; set; }
    }
}
