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
        public UserHub()
        {
            Console.WriteLine("Context.ConnectionId");
        }
        public async Task SendMessage(Message msg)
        {
            if(msg.To == null)
                await Clients.All.SendAsync("ReceiveMessage", msg);
        }
        public async Task Notify()
        {
            await Clients.All.SendAsync("ReceiveNotification");
        }
        public async Task NavigaToRoom(Room room)
        {
            Console.WriteLine(Context.ConnectionId);
            await Clients.All.SendAsync("NavigationToRoom", room);
        }
    }
}
