using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public class Round : TrackedEntity
    {
        public int Id { get; set; }
        public int RoundType { get; set; }
        public int RoundNumber { get; set; }
        public int StartingPlayerId { get; set; }
        public int ActivePlayerId { get; set; }
        public ICollection<Action> Actions { get; set; } = new List<Action>();
        public ICollection<PlayerHand> PlayerHands { get; set; } = new List<PlayerHand>();
        public ICollection<Player> Players { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        public int GameId { get; set; }

        [NotMapped]
        public Action LatestAction => Actions.LastOrDefault();

        [NotMapped]
        public BidAction LatestBid => Actions.OfType<BidAction>().LastOrDefault();

        [NotMapped]
        public LiarAction Liar => Actions.OfType<LiarAction>().LastOrDefault();

        [NotMapped]
        public bool IsCompleted => Liar != null;
    }
}
