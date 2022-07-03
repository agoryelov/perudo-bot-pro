namespace PerudoBot.API.DTOs
{
    public class UserProfileDto : Response
    {
        public string Name { get; set; }
        public ulong DiscordId { get; set; }
        public int Elo { get; set; }
        public int EloRank { get; set; } = 1;
        public int Points { get; set; }
        public int PointsRank { get; set; } = 1;
        public int Score { get; set; }
        public int ScoreRank { get; set; } = 1;
        public List<UserGameDto> RecentGames { get; set; }
        public List<UserAchievementDto> RecentAchievements { get; set; }
    }

    public class UserGameDto
    {
        public int Placing { get; set; }
        public int NetPoints { get; set; }
        public int PlayerCount { get; set; }
    }
}
