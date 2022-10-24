namespace PerudoBot.API.DTOs
{
    public class AuctionDto : Response
    {
        public int AuctionId { get; set; }
        public List<AuctionActionDto> Actions { get; set; }
        public List<AuctionPlayerDto> Players { get; set; }
        public ItemDto Item { get; set; }
        public int ActivePlayerCount { get; set; }
        public int BidCount { get; set; }
        public AuctionActionDto HighestBid { get; set; }
    }

    public class AuctionPlayerDto
    {
        public string Name { get; set; }
        public int PlayerId { get; set; }
        public ulong DiscordId { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
        public int? GamePlayerId { get; set; }
    }

    public class AuctionActionDto
    {
        public int PlayerId { get; set; }
        public bool IsPass { get; set; }
        public int BidAmount { get; set; }
        public string DateCreated { get; set; }
    }

    public class AuctionSummaryDto : Response
    {
        public bool HasWinner { get; set; }
        public int FinalPrice { get; set; }
        public ItemDto Item { get; set; }
        public AuctionPlayerDto Winner { get; set; }
    }
}
