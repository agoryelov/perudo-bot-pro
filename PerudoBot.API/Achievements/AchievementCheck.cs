using PerudoBot.Database.Data;

namespace PerudoBot.API.Achievements
{
    public class AchievementCheck
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public int Type { get; set; }
        public delegate bool Check(Player player, Game game = null, Round round = null);
        public Check Evaluate { get; set; }
    }
}
