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
        public async Task SendMessage(Message msg)
        {
            if(msg.To == null)
                await Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}
