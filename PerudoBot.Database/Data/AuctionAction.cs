using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public abstract class AuctionAction : TrackedEntity
    {
        public int Id { get; set; }

        public string ActionType { get; set; }

        [ForeignKey("AuctionId")]
        public Auction Auction { get; set; }
        public int AuctionId { get; set; }

        [ForeignKey("AuctionPlayerId")]
        public AuctionPlayer AuctionPlayer { get; set; }
        public int AuctionPlayerId { get; set; }

        [ForeignKey("ParentActionId")]
        public AuctionAction ParentAction { get; set; }
        public int? ParentActionId { get; set; }
    }

    public class AuctionBid : AuctionAction
    {
        public int BidAmount { get; set; }
    }

    public class AuctionPass : AuctionAction
    {

    }
}
