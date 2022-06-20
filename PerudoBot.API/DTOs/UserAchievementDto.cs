namespace PerudoBot.API.DTOs
{

    public class AchievementDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int UnlocksCount { get; set; }
        public string UnlockedBy { get; set; }
        public string DateUnlocked { get; set; }
    }

    public class UserAchievementDto 
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DateUnlocked { get; set; }
    }
}
