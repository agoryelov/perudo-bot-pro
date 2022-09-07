using PerudoBot.API.Constants;

namespace PerudoBot.API.DTOs
{
    public class RattleUpdate
    {
        public ulong DiscordId { get; set; }
        public RattleType RattleType { get; set; }
        public RattleContentType RattleContentType { get; set; }
        public string Content { get; set; }
    }
}
