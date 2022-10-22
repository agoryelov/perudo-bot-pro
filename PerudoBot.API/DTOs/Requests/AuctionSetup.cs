namespace PerudoBot.API.DTOs
{
    public class AuctionSetup
    {
        public int ItemId { get; set; }
        public List<ulong> DiscordIds { get; set; }
        public ulong StartingDiscordId { get; set; }
    }
}
