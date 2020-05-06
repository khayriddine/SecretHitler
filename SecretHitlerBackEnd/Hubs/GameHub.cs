using BackEnd.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretHitlerBackEnd.Hubs
{
    public class GameHub : Hub
    {
        //public async Task GameIsReady()
        //{

        //}
        public async Task NewPlayer(Player player)
        {
            player.ConnectionId = Context.ConnectionId;
            await Clients.All.SendAsync("NewPlayerArrived", player);
        }
        public async Task InformNewPlayer(Player player, string newPlayerConnectionId)
        {
            await Clients.Client(newPlayerConnectionId).SendAsync("OldPlayerInfos", player);
        }
        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await Clients.All.SendAsync("PlayerDisconnect", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
