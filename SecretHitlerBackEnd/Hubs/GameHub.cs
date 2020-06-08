using BackEnd.Models;
using Microsoft.AspNetCore.SignalR;
using SecretHitlerBackEnd.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretHitlerBackEnd.Hubs
{
    public class GameHub : Hub
    {
        private int counter = 0;
        private static bool locked;
        private static Dictionary<int,Game> _games;
        private static List<Player> _players;

        public GameHub()
        {
            if (_games == null)
            {
                _games = new Dictionary<int, Game>();
                locked = false;
            }
            if (_players == null)
            {
                _players = new List<Player>();
                locked = false;
            }

        }
        public void NewPlayer(Player player,Room room,bool _new= false)
        {
            lock (_players)
            {
                if(player != null)
                {
                    var pyer = _players.Find(p => p.UserId == player.UserId);
                    if(pyer != null)
                    {
                        pyer.IsDisconnected = false;
                        pyer.ConnectionId = Context.ConnectionId;
                        pyer.RoomId = player.RoomId;
                        pyer.ProfilePicture = player.ProfilePicture; 
                        if (_new)
                        {
                            pyer.IsDead = false;
                        }
                    }
                    else
                    {
                        player.ConnectionId = Context.ConnectionId;
                        _players.Add(player);
                    }
                }
            }
            lock(_games)
            {
                locked = true;
                if (room != null)
                {
                    if (_games == null)
                    {
                        _games = new Dictionary<int, Game>();
                    }
                    if (_games != null && (!_games.ContainsKey(room.RoomId) || _games[room.RoomId].Status == GameStatus.Closed || _new))
                    {
                        Game g = new Game()
                        {
                            GameId = room.RoomId,
                            Status = GameStatus.NotReady,
                            NumberOfRounds = 0,
                            //Players = new List<Player>(),
                            ElectionFailTracker = 0,
                            NbreOfInvestigation = 0,
                            NbreOfKills = 0,
                            NbreOfPeeks = 0,
                            NbreOfPresidentSelection = 0,
                            OnTableCards = new List<Card>(),
                            DiscardedCards = new List<Card>(),
                            TurnState = TurnState.NewTurn
                        };
                        if (!_games.ContainsKey(room.RoomId))
                            _games.Add(room.RoomId, g);
                        else
                            _games[room.RoomId] = g;
                    }
                    _games[room.RoomId].Players = _players.Where(p => p.RoomId == room.RoomId).OrderBy(p => p.Order).ToList();
                    if (_games[room.RoomId].Players.Count == room.NumberOfPlayer)
                    {
                        if (_games[room.RoomId].Status == GameStatus.Paused)
                        {
                            _games[room.RoomId].Status = GameStatus.Ready;
                        }
                        else if (_games[room.RoomId].Status == GameStatus.NotReady)
                        {
                            _games[room.RoomId].Status = GameStatus.Ready;
                            _games[room.RoomId].AssignPlayer();
                        }
                        var cnxs = _players.Where(pyer => pyer.RoomId == player.RoomId).Select(pr => pr.ConnectionId).ToList();
                        Clients.Clients(cnxs).SendAsync("GameUpdated", _games[room.RoomId]);

                    }
                        
                    
                    


                }
            }
           
        }
        public void SendVoteRequest(Player player)
        {
            
            var game = _games[player.RoomId];
            if(game != null)
            {
                lock (game)
                {
                    if (game.TurnVotes == null)
                        game.TurnVotes = new List<Vote>();
                    if (game.TurnVotes.Count() > 0)
                        game.TurnVotes.Clear();
                    game.Chancellor = game.Players.Find(p => p.UserId == player.UserId);
                }
                lock (_players)
                {
                    //var cnxs = _players.Where(pyer => pyer.RoomId == player.RoomId && pyer.IsDead == false).Select(pr => pr.ConnectionId).ToList();
                    var cnxs = _players.Where(pyer => pyer.RoomId == player.RoomId).Select(pr => pr.ConnectionId).ToList();
                    Clients.Clients(cnxs).SendAsync("ReceiveVoteRequest", player);
                }
            }
            
        }
        public void ReplyVoteRequest(Vote vote, int gameId)
        {
            var game = _games[gameId];
            if (game != null)
            {
                lock (game)
                {
                    if (game.TurnVotes == null)
                        game.TurnVotes = new List<Vote>();
                    game.TurnVotes.Add(vote);
                    int nbre_Alive = game.Players.Where(p => p.IsDead == false).Count();
                    if (game.TurnVotes.Count == nbre_Alive)
                    {

                        var cnxs = _players.Where(pyer => pyer.RoomId == gameId).Select(pr => pr.ConnectionId).ToList();
                        //var cnxs = _players.Where(pyer => pyer.RoomId == gameId && pyer.IsDead == false).Select(pr => pr.ConnectionId).ToList();
                        if (game.TurnVotes.Where(v => v.Value == 1).Count() > nbre_Alive / 2)
                        {

                            
                            //reset election tracker
                            game.ElectionFailTracker = 0;
                            //check if fascist can win:
                            if (game.NbreOfFascistCards() >= 3 && game.Chancellor!= null && game.Chancellor.SecretRole == SecretRole.Hitler)
                            {
                                game.WinType = WinType.HitlerSelected;
                                game.Status = GameStatus.Closed;
                                Clients.Clients(cnxs).SendAsync("ReceiveGameResults", game);
                                return;
                            }
                            else
                            {
                                //picks cards:
                                
                            }
                            
                        }
                        else
                        {
                            game.ElectionFailTracker++;
                            game.Chancellor = null;
                        }
                        game.TurnState = TurnState.SameTurn;
                        Clients.Clients(cnxs).SendAsync("ReceiveVoteResults", game.TurnVotes);
                        /*
                        else
                        {
                            
                            
                        }
                        
                        */
                    }
                }
                
                
            }
        }
        public void PresidentDiscard(int discarded, int gameId)
        {
            var game = _games[gameId];
            if(game != null)
            {
                lock (game)
                {
                    if(game.InHandCards != null && game.Chancellor!= null)
                    {
                        var card = game.InHandCards[discarded];
                        game.DiscardedCards.Add(card);
                        game.InHandCards.RemoveAt(discarded);
                        Clients.Client(game.Chancellor.ConnectionId).SendAsync("ReceiveCards", game.InHandCards);
                    }
                }
            }
        }
        public void ChancellorDiscard(int discarded, int gameId)
        {
            var game = _games[gameId];
            var nbre = game.Players.Count;
            var cnxs = _players.Where(pyer => pyer.RoomId == gameId && pyer.IsDead == false).Select(pr => pr.ConnectionId).ToList();
            if (game != null)
            {
                lock (game)
                {
                    if (game.InHandCards != null)
                    {
                        var card = game.InHandCards[discarded];
                        game.DiscardedCards.Add(card);
                        game.InHandCards.RemoveAt(discarded);
                        //finish the turn
                        game.OnTableCards.Add(game.InHandCards[0]);
                        //evaluate the table
                        if(game.NbreOfFascistCards() == 6)
                        {
                            game.WinType = WinType.FascistFull;
                            game.Status = GameStatus.Closed;
                            Clients.Clients(cnxs).SendAsync("ReceiveGameResults", game);
                            return;
                        }
                        else if (game.NbreOfLiberalCards() == 5)
                        {
                            game.WinType = WinType.LiberalFull;
                            game.Status = GameStatus.Closed;
                            Clients.Clients(cnxs).SendAsync("ReceiveGameResults", game);
                            return;
                        }
                        else if (game.NbreOfFascistCards() == 5 && game.NbreOfKills == 1)
                        {
                            game.NbreOfKills++;
                            game.TurnState = TurnState.SameTurn;
                            Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Kill);
                            // offer the kill option
                        }
                        else if (game.NbreOfFascistCards() == 4 && game.NbreOfKills == 0)
                        {
                            game.NbreOfKills++;
                            game.TurnState = TurnState.SameTurn;
                            Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Kill);
                        }
                        else if (game.NbreOfFascistCards() == 3)
                        {
                            
                            if((nbre == 10 || nbre == 9 || nbre == 8 || nbre == 7) && game.NbreOfPresidentSelection == 0)
                            {
                                game.NbreOfPresidentSelection++;
                                game.TurnState = TurnState.SameTurn;
                                Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Select);
                            }
                            else if((nbre == 6 || nbre == 5) && game.NbreOfPeeks == 0)
                            {
                                game.NbreOfPeeks++;
                                game.TurnState = TurnState.SameTurn;
                                Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Peek);
                            }
                            else
                            {
                                game.NumberOfRounds++;
                                game.TurnState = TurnState.NewTurn;
                                while (game.Players[game.NumberOfRounds % game.Players.Count].IsDead == true)
                                {
                                    game.NumberOfRounds++;
                                }
                            }
                        }
                        else if ( game.NbreOfFascistCards() == 2)
                        {
                            if (nbre == 10 || nbre == 9 && game.NbreOfInvestigation == 1)
                            {
                                game.NbreOfInvestigation++;
                                game.TurnState = TurnState.SameTurn;
                                Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Investigate);
                            }
                            else if ((nbre == 8 || nbre == 7) && game.NbreOfInvestigation == 0)
                            {
                                game.NbreOfInvestigation++;
                                game.TurnState = TurnState.SameTurn;
                                Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Investigate);
                            }
                            else
                            {
                                game.NumberOfRounds++;
                                while (game.Players[game.NumberOfRounds % game.Players.Count].IsDead == true)
                                {
                                    game.NumberOfRounds++;
                                }
                                game.TurnState = TurnState.NewTurn;
                            }

                        }
                        else if (game.NbreOfFascistCards() == 1 && (nbre == 10 || nbre == 9) && game.NbreOfInvestigation == 0)
                        {
                                game.NbreOfInvestigation++;
                            game.TurnState = TurnState.SameTurn;
                            Clients.Clients(game.ActualPlayer().ConnectionId).SendAsync("ExecutePower", Power.Investigate);
                          
                        }
                        else
                        {
                            game.NumberOfRounds++;
                            while (game.Players[game.NumberOfRounds % game.Players.Count].IsDead == true)
                            {
                                game.NumberOfRounds++;
                            }
                            game.TurnState = TurnState.NewTurn;
                        }
                        Clients.Clients(cnxs).SendAsync("GameUpdated", game);

                    }
                }
            }
        }
        public void PickCards(int gameId)
        {
            var game = _games[gameId];
            if (game != null)
            {
                lock (game)
                {
                    if (game.RemainingCards.Count() < 3 && game.DiscardedCards != null)
                    {
                        game.RemainingCards.AddRange(game.DiscardedCards);
                        game.RemainingCards.Shuffle();
                        game.DiscardedCards.Clear();
                    }
                        
                    game.InHandCards = game.RemainingCards.GetRange(0, 3);
                    game.RemainingCards.RemoveRange(0, 3);
                    Clients.Client(game.ActualPlayer().ConnectionId).SendAsync("ReceiveCards", game.InHandCards);

                }
            }
        }
        public void RequestPeek(int gameId,string cnx)
        {
            var game = _games[gameId];
            if (game != null)
            {
                lock (game)
                {
                    if (game.RemainingCards.Count() < 3 && game.DiscardedCards != null)
                    {
                        game.RemainingCards.AddRange(game.DiscardedCards);
                        game.RemainingCards.Shuffle();
                        game.DiscardedCards.Clear();
                    }
                    var cards = game.RemainingCards.GetRange(0, 3);
                    Clients.Client(cnx).SendAsync("PeekCards", cards);

                }
            }
        }
        public void KillRequest(int deadId,int gameId)
        {
            var hitlerKilled = false;
            //var cnxs = _players.Where(pyer => pyer.RoomId == gameId && pyer.IsDead == false).Select(pr => pr.ConnectionId).ToList();
            var cnxs = _players.Where(pyer => pyer.RoomId == gameId).Select(pr => pr.ConnectionId).ToList();
            lock (_players)
            {
                var p = _players.Find(per => per.UserId == deadId);
                if (p != null)
                    p.IsDead = true;
                if(p.SecretRole == SecretRole.Hitler)
                {
                    hitlerKilled = true;
                    
                }
            }
            var game = _games[gameId];
            if (game != null)
            {
                lock (game)
                {
                    if (hitlerKilled)
                    {
                        game.WinType = WinType.HitlerDead;
                        game.Status = GameStatus.Closed;
                        Clients.Clients(cnxs).SendAsync("ReceiveGameResults", game);
                        return;
                    }
                    game.NumberOfRounds++;
                    while (game.Players[game.NumberOfRounds%game.Players.Count].IsDead == true)
                    {
                        game.NumberOfRounds++;
                    }
                    game.TurnState = TurnState.NewTurn;
                    Clients.Clients(cnxs).SendAsync("GameUpdated", game);

                }
            }
        }
        public void SelectPresidentRequest(int chosenId, int gameId)
        {
            var cnxs = _players.Where(pyer => pyer.RoomId == gameId && pyer.IsDead == false).Select(pr => pr.ConnectionId).ToList();

            var game = _games[gameId];
            if (game != null)
            {
                lock (game)
                {
                    var pi = game.Players.FindIndex(per => per.UserId == chosenId);
                    while (((game.NumberOfRounds) % game.Players.Count) != ((pi) % game.Players.Count))
                    {
                        game.NumberOfRounds++;
                    }
                    game.TurnState = TurnState.NewTurn;
                    Clients.Clients(cnxs).SendAsync("GameUpdated", game);

                }
            }
        }
        public void FinishTurn(int gameId)
        {
            //var cnxs = _players.Where(pyer => pyer.RoomId == gameId && pyer.IsDead == false).Select(pr => pr.ConnectionId).ToList();
            var cnxs = _players.Where(pyer => pyer.RoomId == gameId).Select(pr => pr.ConnectionId).ToList();

            var game = _games[gameId];
            if (game != null)
            {
                lock (game)
                {
                    if (game.ElectionFailTracker == 3)
                    {
                        var card = game.RemainingCards[0];

                        game.RemainingCards.Remove(card);
                        game.OnTableCards.Add(card);
                        if(card.CardType == CardType.Fascist)
                        {
                            if(game.NbreOfFascistCards() == 3)
                            {
                                game.NbreOfPeeks++;
                            }else if(game.NbreOfFascistCards() >=  4)
                            {
                                game.NbreOfKills++;
                            }
                        }
                        game.ElectionFailTracker = 0;
                    }
                    game.NumberOfRounds++;
                    while(game.ActualPlayer().IsDead == true)
                    {
                        game.NumberOfRounds++;
                    }
                    game.TurnState = TurnState.NewTurn;
                    Clients.Clients(cnxs).SendAsync("GameUpdated", game);

                }
            }
        }
        //
        public void ClearCache()
        {
            lock (_games)
            {
                _games.Clear();
            }
            lock (_players)
            {
                _players.Clear();
            }
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
    

        public async Task Notify(string notif, List<string> connections)
        {
            await Clients.Clients(connections).SendAsync("ReceiveNotif", notif);
        }
        public async Task VoteResult(List<Vote> votes, List<string> connections)
        {
            await Clients.Clients(connections).SendAsync("ReceiveVoteResults", votes);
        }
        public override async Task OnDisconnectedAsync(System.Exception exception)//votes:Vote[],connections:string[]
        {
            lock (_players)
            {
                var p = _players.Find(pyer => pyer.ConnectionId == Context.ConnectionId);

                p.IsDisconnected = true;
                var cnxs = _players.Where(pyer => pyer.RoomId == p.RoomId).Select(pr => pr.ConnectionId).ToList();
                Clients.Clients(cnxs).SendAsync("ReceiveNotif", "Player "+p.Name+" disconnected");
                
            }
            await base.OnDisconnectedAsync(exception);
        }

    }
}
