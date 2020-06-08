using BackEnd.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretHitlerBackEnd.Hubs
{
    public class UserHub : Hub
    {
        static Dictionary<string, int> usersHub;
        static Dictionary<int, Room> _rooms;
        static int nbreOfRooms;
        public UserHub()
        {

            if (usersHub == null)
                usersHub = new Dictionary<string, int>();
            if(_rooms == null)
            {
                _rooms = new Dictionary<int, Room>();
                nbreOfRooms = 0;
            }
        }
        public async Task SendMessage(Message msg)
        {
            if(msg.To == null)
                await Clients.All.SendAsync("ReceiveMessage", msg);
        }
        public async Task Notify(string type)
        {
            await Clients.All.SendAsync("ReceiveNotification",type);
        }
        public async Task SendRooms(List<Room> rooms)
        {
            await Clients.All.SendAsync("GetRooms", rooms);
        }
        public void RequestRooms()
        {
             Clients.All.SendAsync("GetRooms", _rooms.Values.ToList());
        }
        public void NewUser(int userId)
        {
            if (usersHub == null)
                usersHub = new Dictionary<string, int>();
            usersHub[Context.ConnectionId] = userId;
            Clients.Client(Context.ConnectionId).SendAsync("GetRooms", _rooms.Values.ToList());
        }
        public void CreateRoom(Room room)
        {
            if(room != null && _rooms != null)
            {
                lock (_rooms)
                {
                    room.RoomId = nbreOfRooms;
                    _rooms.Add(nbreOfRooms++,room);
                }
                Clients.All.SendAsync("UpdateRoom", room);

            }
        }
        public void RemoveRoom(int RoomId)
        {
            if (_rooms != null)
            {
                lock (_rooms)
                {
                    if (_rooms.ContainsKey(RoomId))
                        _rooms.Remove(RoomId);
                }
                Clients.All.SendAsync("GetRooms", _rooms.Values.ToList());
            }
        }
        public void JoinRoom(User user,int roomId)
        {
            var room = _rooms[roomId];
            if (room != null)
            {
                lock (room)
                {
                    room.UsersJoining.Add(user);
                }
                Clients.All.SendAsync("GetRooms", _rooms.Values.ToList());
            }
            
        }
        public void LeaveRoom(int UserId,int RoomId)
        {
            var room = _rooms[RoomId];
            bool left = false;
            if (_rooms.ContainsKey(RoomId))
            {
                lock (_rooms)
                {
                    if (_rooms[RoomId].UsersJoining != null)
                    {
                        int i = _rooms[RoomId].UsersJoining.FindIndex(u => u.UserId == UserId);
                        if(i >= 0)
                        {
                            _rooms[RoomId].UsersJoining.RemoveAt(i);
                            left = true;
                        }
                            
                    }
                        
                    
                }
                if(left == true)
                    Clients.All.SendAsync("GetRooms", _rooms.Values.ToList());
            }

        }
        public void NavigaToRoom(Room room)
        {
            if(usersHub != null && room != null && room.UsersJoining != null)
            {
                foreach(var us in usersHub)
                {
                    
                    if(room.UsersJoining.Where(u => u.UserId == us.Value).Count() > 0)
                        Clients.Client(us.Key).SendAsync("NavigationToRoom", room);
                }
            }
        }
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("GetRooms", _rooms.Values.ToList());
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            usersHub.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
