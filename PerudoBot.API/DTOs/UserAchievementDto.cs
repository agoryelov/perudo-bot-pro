namespace PerudoBot.API.DTOs
{
    public class AchievementDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
    }

    public class AchievementDetailsDto : AchievementDto
    {
        public List<string> UnlockedBy { get; set; }
        public int UnlocksCount => UnlockedBy.Count;

        public AchievementDetailsDto(AchievementDto achievementDto)
        {
            Name = achievementDto.Name;
            Description = achievementDto.Description;
            Score = achievementDto.Score;
        }
    }

    public class UserAchievementDto : AchievementDto
    {
        public string Username { get; set; }
        public string DateUnlocked { get; set; }

        public UserAchievementDto(AchievementDto achievementDto)
        {
            Name = achievementDto.Name;
            Description = achievementDto.Description;
            Score = achievementDto.Score;
        }
    }
}
