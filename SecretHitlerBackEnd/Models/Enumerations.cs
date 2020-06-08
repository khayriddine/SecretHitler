using System;
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
        Liberal, Fascist, Hitler
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
        //Idle, Ongoing, Closed
        NotReady = 0,Ready = 1,
        Paused = 2, Closed = 3
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
        None=0, Friends=1, Sending=2,Pending=3
    }
    public enum RequestAction
    {
        None=0,Send = 1, Accept =2, Decline=3
    }
    public enum CardType
    {
        Liberal = 0, Fascist = 1
    }
    public enum Power
    {
        Investigate = 0, Peek = 1, Select = 2, Kill = 3
    }
    public enum WinType
    {
        HitlerDead = 0, LiberalFull = 1,HitlerSelected=2,FascistFull=3
    }
    public enum TurnState
    {
        SameTurn =0,NewTurn =1,
    }

}
