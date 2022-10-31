using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public class Auction : TrackedEntity
    {
        public int Id { get; set; }
        public int State { get; set; }
        public int Day { get; set; }

        [ForeignKey("AuctionItemId")]
        public Item AuctionItem { get; set; }
        public int AuctionItemId { get; set; }

        public ICollection<AuctionPlayer> AuctionPlayers { get; set; }
        public ICollection<AuctionAction> AuctionActions { get; set; }

        public int WinningPlayerId { get; set; }
        public int FinalPrice { get; set; } 

        public ICollection<User> Users { get; set; }

        [NotMapped]
        public ICollection<AuctionPlayer> ActivePlayers => AuctionPlayers.Where(x => x.IsActive).ToList();

        [NotMapped]
        public ICollection<AuctionPlayer> PassedPlayers => AuctionPlayers.Where(x => !x.IsActive).ToList();

        [NotMapped]
        public AuctionBid HighestBid => AuctionActions.OfType<AuctionBid>()?.OrderByDescending(x => x.BidAmount).FirstOrDefault();

        [NotMapped]
        public AuctionAction LatestAction => AuctionActions.LastOrDefault();

        [NotMapped]
        public bool IsCompleted => AuctionPlayers.Count() == 1;

    }
}
