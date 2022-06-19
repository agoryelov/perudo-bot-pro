namespace PerudoBot.API.DTOs
{
    public class DiscordUser
    {
        public ulong DiscordId { get; set; }
        public string Name { get; set; }
        public bool IsBot { get; set; }
    }
}
