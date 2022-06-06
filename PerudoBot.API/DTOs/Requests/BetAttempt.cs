using PerudoBot.API.Constants;

namespace PerudoBot.API.DTOs
{
    public class BetAttempt
    {
        public BetType Type { get; set; }
        public int Amount { get; set; }
    }
}
