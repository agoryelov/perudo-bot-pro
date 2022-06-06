using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public abstract class Action : TrackedEntity
    {
        public int Id { get; set; }
        public string ActionType { get; set; }

        [ForeignKey("RoundId")]
        public Round Round { get; set; }
        public int RoundId { get; set; }


        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        public int PlayerId { get; set; }


        [ForeignKey("PlayerHandId")]
        public PlayerHand PlayerHand { get; set; }
        public int? PlayerHandId { get; set; }

        [ForeignKey("ParentActionId")]
        public Action ParentAction { get; set; }
        public int? ParentActionId { get; set; }
    }

    public class BidAction : Action
    {
        public int Quantity { get; set; }
        public int Pips { get; set; }
    }

    public class LiarAction : Action
    {
        public int LosingPlayerId { get; set; }
        public int LivesLost { get; set; }
        public int ActualQuantity { get; set; }

        [NotMapped]
        public bool IsSuccessful => LosingPlayerId != PlayerId;

        [NotMapped]
        public BidAction TargetBid => Round.LatestBid;
    }

    public class BetAction : Action
    {
        public int BetAmount { get; set; }
        public int BetType { get; set; }

        public double BetOdds { get; set; }

        public bool IsSuccessful { get; set; }

        [ForeignKey("TargetBidId")]
        public BidAction TargetBid { get; set; }

        public int TargetBidId { get; set; }

        [NotMapped]
        public int BetQuantity => TargetBid.Quantity;

        [NotMapped]
        public int BetPips => TargetBid.Pips;
    }
}
