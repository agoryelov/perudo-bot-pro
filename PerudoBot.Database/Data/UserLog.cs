using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public abstract class UserLog : TrackedEntity
    {
        public int Id { get; set; }
        public string UserLogType { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        public int? GameId { get; set; }

        public Auction Auction { get; set; }
        public int? AuctionId { get; set; }
    }

    public class EloLog : UserLog
    {
        public int EloChange { get; set; }
    }

    public class PointsLog : UserLog
    {
        public int PointsChange { get; set; }
        public int PointsLogTypeId { get; set; }
    }
}
