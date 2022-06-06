namespace PerudoBot.API.DTOs
{
    public class UserAchievementDto
    {
        public string UserName { get; set; }
        public string AchievementName { get; set; }
        public string AchievementDescription { get; set; }
        public DateTime DateUnlocked { get; set; }
    }
}
