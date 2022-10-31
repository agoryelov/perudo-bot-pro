namespace PerudoBot.API.DTOs
{
    public class GameSetupDto : Response
    {
        public int GameId { get; set; }
        public List<PlayerDto> Players { get; set; }
        public int DefaultRoundType { get; set; }
        public AuctionSetupDto AuctionSetup { get; set; }
    }
}
