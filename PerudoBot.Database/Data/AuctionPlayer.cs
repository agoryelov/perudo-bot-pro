using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public class AuctionPlayer
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public int? GamePlayerId { get; set; }

        [ForeignKey("AuctionId")]
        public Auction Auction { get; set; }
        public int AuctionId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
