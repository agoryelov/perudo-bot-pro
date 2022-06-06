namespace PerudoBot.API.DTOs
{
    public class GameDto : Response
    {
        public int GameId { get; set; }
        public int WinnningPlayerId { get; set; }
        public List<PlayerEloChange> EloChanges { get; set; }
        public List<PlayerPointsChange> BetPointsChanges { get; set; }
        public List<GameNote> Notes { get; set; }
        public List<UserAchievementDto> Achievements { get; set; }
    }

    public class PlayerPointsChange {
        public string Name { get; set; }
        public int StartingPoints { get; set; }
        public int FinalPoints { get; set; }
        public int PointsChange => FinalPoints - StartingPoints;
    }

    public class PlayerEloChange
    {
        public string Name { get; set; }
        public int StartingElo { get; set; }
        public int FinalElo { get; set; }
        public int EloChange => FinalElo - StartingElo;
    }

    public class GameNote
    {
        public int RoundNumber { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }
}
