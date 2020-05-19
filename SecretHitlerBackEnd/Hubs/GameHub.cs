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
        public async Task SendCards(Card[] cards,string receiver,string sender)
        {
            await Clients.Client(receiver).SendAsync("ReceiveCards", cards, sender);
        }
        public async Task SendResponse(int response, string receiver)
        {
            await Clients.Client(receiver).SendAsync("ReceiveResponse", response);
        }
        public async Task GoNextTurn(Game game,string receiver)
        {
            await Clients.Client(receiver).SendAsync("GameUpdated", game);
        }
        public async Task BroadcastGameStatus(Game game)
        {
            await Clients.Others.SendAsync("GameStatusUpdated", game);
        }
        public async Task BroadcastToSomeGameStatus(Game game,List<string> clients)
        {
            await Clients.Clients(clients).SendAsync("GameStatusUpdated", game);
        }
        public async Task SendSignal(SignalInfo signal)
        {
            await Clients.Others.SendAsync("ReceiveSignal", signal);
        }
        public async Task SendVoteRequest(Player player,List<string> players)
        {
            await Clients.Clients(players).SendAsync("ReceiveVoteRequest",player,Context.ConnectionId);
        }
        public  async Task ReplyVoteRequest(Vote vote,string receiver)
        {
            await Clients.Client(receiver).SendAsync("ReceiveVote", vote);
        }
        public async Task Notify(string notif, List<string> connections)
        {
            await Clients.Clients(connections).SendAsync("ReceiveNotif", notif);
        }
        public override async Task OnDisconnectedAsync(System.Exception exception)//GoNextTurn
        {
            await Clients.All.SendAsync("PlayerDisconnect", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
