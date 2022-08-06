namespace PerudoBot.API.Constants
{
    public static class GameConstants
    {
        public const int STARTING_LIVES = 5;
        public const int POINTS_EARNED_PER_ROUND = 10;

        public const int RECENT_GAMES = 5;
        public const int RECENT_ACHIEVEMENTS = 3;
        public const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:sszzz";

        public const float ZERO_ODDS = 1.0f;
        public const float PITY_ODDS = 1.1f;
        public const float LIAR_ODDS = 1.5f;
        public const float EXACT_ODDS = 5.0f;
        public const float PEAK_ODDS = 9.0f;
        public const float LEGIT_ODDS = 2.0f;

        public static int MaxBetPerDie(this BetType betType)
        {
            switch (betType)
            {
                case BetType.Exact: return 50;
                case BetType.Peak: return 50;
                case BetType.Liar: return 50;
                case BetType.Legit: return 50;
                default: throw new ArgumentOutOfRangeException("betType");
            }
        }
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
        Liar = 1,
        Peak = 2,
        Legit = 3
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
