namespace PerudoBot.API.DTOs
{
    public class PlayerDto : Response
    {
        public int PlayerId { get; set; }
        public ulong DiscordId { get; set; }
        public bool IsBot { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Lives { get; set; }
        public string EquippedDice { get; set; }
        public List<int> Dice { get; set; } = new List<int>();
        public List<RattleDto> Rattles { get; set; }
    }

    public class RattleDto
    {
        public int RattleType { get; set; }
        public int RattleContentType { get; set; }
        public string Content { get; set; }
    }
}
