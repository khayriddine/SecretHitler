﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public BoardSize BoardSize { get; set; }
        public int NumberOfPlayer { get; set; }
        public GameAcess GameAcess { get; set; }

        public ICollection<User> UsersJoining { get; set; }

    }
}