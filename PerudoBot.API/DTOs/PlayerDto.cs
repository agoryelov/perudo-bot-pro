namespace PerudoBot.API.DTOs
{
    public class PlayerDto : Response
    {
        public int PlayerId { get; set; }
        public ulong DiscordId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Lives { get; set; }
        public List<int> Dice { get; set; } = new List<int>();
    }
}
