namespace PerudoBot.API.DTOs
{
    public class ActionDto
    {
        public int PlayerId { get; set; }
        public string ActionType { get; set; }
    }

    public class BidDto : ActionDto
    {
        public int Quantity { get; set; }
        public int Pips { get; set; }
        public string DateCreated { get; set; }
    }

    public class LiarDto : ActionDto
    {
        public BidDto TargetBid { get; set; }
        public int LosingPlayerId { get; set; }
        public int LivesLost { get; set; }
        public int ActualQuantity { get; set; }
        public bool IsSuccessful { get; set; }
    }

    public class BetDto : ActionDto
    {
        public BidDto TargetBid { get; set; }
        public double BetOdds { get; set; }
        public bool IsSuccessful { get; set; }
        public int BetAmount { get; set; }
        public int BetType { get; set; }
    }
}
