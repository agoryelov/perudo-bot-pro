using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PerudoBot.Database.Data
{
    public class User
    {
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public bool IsBot { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Elo { get; set; }
        public int AchievementScore { get; set; }

        public ICollection<Rattle> Rattles { get; set; } = new List<Rattle>();
        public ICollection<Game> Games { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; }
        public ICollection<Achievement> Achievements { get; set; }

        public ICollection<UserLog> UserLogs { get; set; }

        public ICollection<UserItem> UserItems { get; set; }
        public DiceItem EquippedDice { get; set; }
    }
}
