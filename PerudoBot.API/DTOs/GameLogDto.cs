namespace PerudoBot.API.DTOs
{
    public class GameLogDto : Response
    {
        public List<LogEntryDto> LogEntries { get; set; }
    }

    public class LogEntryDto
    {
        public int GameId { get; set; }
        public string GameState { get; set; }
        public string GameDate { get; set; }
    }
}
