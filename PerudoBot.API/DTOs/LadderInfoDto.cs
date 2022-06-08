namespace PerudoBot.API.DTOs
{
    public class LadderInfoDto : Response
    {
        public List<LadderEntryDto> LadderEntries { get; set; }
    }

    public class LadderEntryDto
    {
        public string Name { get; set; }
        public int Elo { get; set; }
        public int Points { get; set; }
        public int GamesPlayed { get; set; }
    }
}
