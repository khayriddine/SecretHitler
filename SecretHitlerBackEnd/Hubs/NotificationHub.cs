using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretHitlerBackEnd.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task BroadcastNotifications(List<string>  receivers,string content)
        {
            await Clients.Clients(receivers).SendAsync("ReceiveNotification",content);
        }
    }
}
