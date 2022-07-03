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
                Name = "Fate Worse Than Death",
                Description = "Lose all points in a single round",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    return player.User.Points == 0;
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
                Description = "Take 4 damage first round in a reverse game with 5 or more players",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    if (round.RoundNumber != 1) return false;
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    if (game.Players.Count < 5) return false;
                    if (round.Liar.LosingPlayerId != player.Id) return false;
                    return round.Liar.LivesLost == 4;
                }
            },
            new AchievementCheck
            {
                Name = "Frequent Liar",
                Description = "Bluff 4 bids in a row in a single game",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    var bids = game.Rounds.SelectMany(x => x.Actions).OfType<BidAction>().Where(x => x.PlayerId == player.Id).TakeLast(4);
                    if (bids.Count() < 4) return false;
                    return bids.All(x => x.IsBluff());
                }
            },
            new AchievementCheck
            {
                Name = "Stone Cold",
                Description = "Bluff 8 bids in a row in a single game",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.ExtraHard,
                Evaluate = (player, game, round) =>
                {
                    var bids = game.Rounds.SelectMany(x => x.Actions).OfType<BidAction>().Where(x => x.PlayerId == player.Id).TakeLast(8);
                    if (bids.Count() < 8) return false;
                    return bids.All(x => x.IsBluff());
                }
            },
            new AchievementCheck
            {
                Name = "Style Points",
                Description = "Win 777 points in a single round",
                Type = (int)AchievementType.Round,
                Score = (int)AchievementScore.ExtraHard,
                Evaluate = (player, game, round) =>
                {
                    var bet = round.Actions.OfType<BetAction>().FirstOrDefault(x => x.PlayerId == player.Id);
                    if (bet == null) return false;
                    return bet.WinAmount() == 777;
                }
            },
        };
    }
}
