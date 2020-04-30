﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public enum Gender
    {
        Male, Female
    }
    public enum SecretRole
    {
        Liberal, Fascist, SecretHitler
    }
    public enum PublicRole
    {
        Member, President
    }
    public enum Status
    {
        Online, Offline, Away, Spectating
    }
    public enum BoardSize
    {
        Small, Medium, Large
    }
    public enum GameStatus
    {
        Idle, Ongoing, Closed
    }
    public enum GameAcess
    {
        FriendsOnly, All, WithToken
    }
    public enum Role
    {
        Admin, Member, Spectator
    }
    public enum RelationshipStatus
    {
        None, Friends
    }
}