using SecretHitlerBackEnd.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Game
    {
        public Game()
        {
            newGameCards();
        }

        public int GameId { get; set; }
        //public string Name { get; set; }
        //public string Password { get; set; }
        public GameStatus Status { get; set; }
        public int ElectionFailTracker { get; set; }
        public int NbreOfPeeks { get; set; }
        public int NbreOfKills { get; set; }
        public int NbreOfInvestigation { get; set; }
        public int NbreOfPresidentSelection { get; set; }
        public Player Chancellor { get; set; }
        public WinType WinType { get; set; }
        public string chosen { get; set; }
        public TurnState TurnState { get; set; }
        //public GameAcess GameAcess { get; set; }

        //public int BoardId { get; set; }
        //public ICollection<User> UsersPlaying { get; set; }
        //public ICollection<User> UsersWatching { get; set; }
        //public Board Board { get; set; }

        public int NumberOfRounds { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> RemainingCards { get; set; }
        public List<Card> DiscardedCards { get; set; }
        public List<Card> InHandCards { get; set; }
        public List<Card> OnTableCards { get; set; }
        public List<Vote> TurnVotes { get; set; }

        public Player ActualPlayer()
        {
            if (Players == null)
                return null;
            return Players[NumberOfRounds % Players.Count];
        }
        public int NbreOfFascistCards()
        {
            if (OnTableCards == null)
                return -1;
            return OnTableCards.Where(c => c.CardType == CardType.Fascist).Count();
        }
        public int NbreOfLiberalCards()
        {
            if (OnTableCards == null)
                return -1;
            return OnTableCards.Where(c => c.CardType == CardType.Liberal).Count();
        }
        public void newGameCards()
        {
            if (RemainingCards == null)
                RemainingCards = new List<Card>();
            else
                RemainingCards.Clear();
            RemainingCards.AddRange(new Card[]{
                new Card(CardType.Fascist),
                new Card(CardType.Fascist),
                new Card(CardType.Liberal),
                new Card(CardType.Fascist),
                new Card(CardType.Liberal),
                new Card(CardType.Fascist),
                new Card(CardType.Fascist),
                new Card(CardType.Fascist),
                new Card(CardType.Liberal),
                new Card(CardType.Fascist),
                new Card(CardType.Liberal),
                new Card(CardType.Liberal),
                new Card(CardType.Fascist),
                new Card(CardType.Fascist),
                new Card(CardType.Fascist),
                new Card(CardType.Liberal),
                new Card(CardType.Fascist)
                });
            RemainingCards.Shuffle();
        }
        public void AssignPlayer()
        {
            Players.Shuffle();
            var roles = new List<SecretRole>();
            switch (Players.Count)
            {
                case 5:
                {
                    for (int i = 0; i < 3; i++)
                        roles.Add(SecretRole.Liberal);
                        roles.Add(SecretRole.Hitler);
                        roles.Add(SecretRole.Fascist);

                }break;
                case 6:
                {
                for (int i = 0; i < 4; i++)
                        roles.Add(SecretRole.Liberal);
                    roles.Add(SecretRole.Hitler);
                    roles.Add(SecretRole.Fascist);

                }
                break;
                case 7:
                {
                    for (int i = 0; i < 4; i++)
                            roles.Add(SecretRole.Liberal);
                        roles.Add(SecretRole.Hitler);
                        roles.Add(SecretRole.Fascist);
                        roles.Add(SecretRole.Fascist);

                    }
                break;
                case 8:
                {
                    for (int i = 0; i < 5; i++)
                            roles.Add(SecretRole.Liberal);
                        roles.Add(SecretRole.Hitler);
                        roles.Add(SecretRole.Fascist);
                        roles.Add(SecretRole.Fascist);

                    }
                break;
                case 9:
                {
                    for (int i = 0; i < 5; i++)
                            roles.Add(SecretRole.Liberal);
                        roles.Add(SecretRole.Hitler);
                        roles.Add(SecretRole.Fascist);
                        roles.Add(SecretRole.Fascist);
                        roles.Add(SecretRole.Fascist);

                    }
                break;
                case 10:
                {
                    for (int i = 0; i < 6; i++)
                            roles.Add(SecretRole.Liberal);
                        roles.Add(SecretRole.Hitler);
                        roles.Add(SecretRole.Fascist);
                        roles.Add(SecretRole.Fascist);
                        roles.Add(SecretRole.Fascist);

                }
                break;
            }
            if(this.Players!= null)
            {
                roles.Shuffle();
                for (int i = 0; i < Players.Count; i++)
                {
                    Players[i].SecretRole = roles[i];
                    Players[i].Order = i;
                }
                    
            }
        }
    }
}
