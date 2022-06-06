namespace PerudoBot.API.DTOs
{
    public class RoundSummaryDto : Response
    {
        public RoundDto Round { get; set; }
        public List<UserAchievementDto> Achievements { get; set; }
    }
}
