namespace PerudoBot.Database.Data
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<UserAchievement> UserAchievements { get; set; }
    }
}
