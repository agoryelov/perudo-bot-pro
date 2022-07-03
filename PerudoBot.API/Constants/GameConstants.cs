namespace PerudoBot.API.Constants
{
    public static class GameConstants
    {
        public const int STARTING_LIVES = 5;
        public const int POINTS_EARNED_PER_ROUND = 10;
        public const float MAX_BET_ODDS = 4.0f;
        public const float MIN_BET_ODDS = 1.5f;
        public const int RECENT_GAMES = 10;
        public const int RECENT_ACHIEVEMENTS = 3;
        public const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:sszzz";
    }

    public enum GameState
    {
        Setup = 0,
        InProgress = 1,
        Terminated = 2,
        Ended = 3
    }

    public enum RoundType
    {
        SuddenDeath = 0,
        Reverse = 1
    }

    public enum BetType
    {
        Exact = 0,
        Liar = 1
    }

    public enum AchievementType
    {
        Round = 0,
        Game = 1,
        Lifetime = 2,
        Seasonal = 3
    }

    public enum AchievementScore
    {
        Easy = 100,
        Medium = 250,
        Hard = 500,
        ExtraHard = 750
    }
}
