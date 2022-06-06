namespace PerudoBot.API.DTOs
{
    public class RoundDto : Response
    {
        public int ActivePlayerId { get; set; }
        public int RoundNumber { get; set; }
        public int RoundType { get; set; }
        public int TotalDiceCount { get; set; }
        public int ActivePlayerCount { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<BetDto> Bets { get; set; }
        public BidDto LatestBid { get; set; }
        public LiarDto Liar { get; set; }
    }
}
