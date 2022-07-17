using PerudoBot.API.Constants;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Achievements
{
    public static partial class AchievementData
    {
        public static List<AchievementCheck> RoundAchievements = new List<AchievementCheck>
        {
            new AchievementCheck
            {
                Name = "Rules Are Rules",
                Description = "Correctly call a quick maths six bid at the start of the round",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Easy,
                Evaluate = (player, game, round) =>
                {
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (!round.Liar.IsSuccessful) return false;
                    if (round.Actions.OfType<BidAction>().Count() != 1) return false;
                    var firstBid = round.Actions.OfType<BidAction>().First();
                    if (firstBid.Pips != 6) return false;
                    var quickMaths = (int) Math.Round(round.PlayerHands.GetAllDice().Count / 3.0);
                    return firstBid.Quantity == quickMaths;
                }
            },
            new AchievementCheck
            {
                Name = "Ice Cold",
                Description = "Correctly call liar on a bid under quick maths",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Easy,
                Evaluate = (player, game, round) =>
                {
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (!round.Liar.IsSuccessful) return false;
                    if (round.Liar.TargetBid.Pips == 1) return false;
                    var quickMaths = (int) Math.Round(round.PlayerHands.GetAllDice().Count / 3.0);
                    return round.Liar.TargetBid.Quantity < quickMaths;
                }
            },
            new AchievementCheck
            {
                Name = "A Lot of Damage",
                Description = "Deal 5 or more damage with a single liar call in Reverse",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (!round.Liar.IsSuccessful) return false;
                    return round.Liar.LivesLost >= 5;
                }
            },
            new AchievementCheck
            {
                Name = "Just Wanted Info",
                Description = "Take 4 damage at full life in a reverse game with 5 or more players",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    if (game.Players.Count < 5) return false;
                    if (round.Liar.LosingPlayerId != player.Id) return false;
                    return round.Liar.LivesLost == 4;
                }
            },
            new AchievementCheck
            {
                Name = "Money to Burn",
                Description = "Win 1000 or more points in a single Legit bet",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Hard,
                Evaluate = (player, game, round) =>
                {
                    var bet = round.Actions.OfType<BetAction>().FirstOrDefault(x => x.PlayerId == player.Id && x.BetType == (int)BetType.Legit);
                    if (bet == null) return false;
                    return (bet.WinAmount() - bet.BetAmount) >= 1000;
                }
            },
            new AchievementCheck
            {
                Name = "Jackpot",
                Description = "Win 5000 or more points in a single round",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.ExtraHard,
                Evaluate = (player, game, round) =>
                {
                    var bet = round.Actions.OfType<BetAction>().FirstOrDefault(x => x.PlayerId == player.Id);
                    if (bet == null) return false;
                    return (bet.WinAmount() - bet.BetAmount) >= 5000;
                }
            }
        };
    }
}
