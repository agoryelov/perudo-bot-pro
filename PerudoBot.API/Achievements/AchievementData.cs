using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;
using PerudoBot.API.Constants;

namespace PerudoBot.API.Achievements
{
    public static class AchievementData
    {
        public static List<AchievementCheck> RoundAchievements = new List<AchievementCheck>
        {
            new AchievementCheck
            {
                Name = "Rules Are Rules",
                Description = "Correctly call a quick maths six bid at the start of the round",
                Type = (int)AchievementType.Round,
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
                Name = "Chasing Risk",
                Description = "Correclty call liar out of turn",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (!round.Liar.IsSuccessful) return false;
                    if (round.ActivePlayerId == player.Id) return false;
                    else return true;
                }
            },
            new AchievementCheck
            {
                Name = "Fate Worse Than Death",
                Description = "Lose 5,000 Points in a single round",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    var playerBet = round.Actions.OfType<BetAction>().Where(x => x.PlayerId == player.Id).FirstOrDefault();
                    if (playerBet == null) return false;
                    if (playerBet.IsSuccessful) return false;
                    return playerBet.BetAmount >= 5000;
                }
            },
            new AchievementCheck
            {
                Name = "Ice Cold",
                Description = "Correctly call liar on a bid under quick maths",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (!round.Liar.IsSuccessful) return false;
                    var quickMaths = (int) Math.Round(round.PlayerHands.GetAllDice().Count / 3.0);
                    return round.Liar.TargetBid.Quantity < quickMaths;
                }
            },
            new AchievementCheck
            {
                Name = "A Lot of Damage",
                Description = "Deal 5 or more damage with a single liar call in Reverse",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (!round.Liar.IsSuccessful) return false;
                    return round.Liar.LivesLost >= 5;
                }
            }
        };

        public static List<AchievementCheck> GameAchievements = new List<AchievementCheck>
        {
            new AchievementCheck
            {
                Name = "Not a Scratch",
                Description = "Win a game with 5 or more players with 5 lives remaining",
                Type = (int)AchievementType.Game,
                Evaluate = (player, game, round) => 
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    if (game.Players.Count < 5) return false;
                    return game.ActivePlayers.First().Lives == 5;
                }
            },
            new AchievementCheck
            {
                Name = "Peaceful Victory",
                Description = "Win a game with 5 or more players without calling liar",
                Type = (int)AchievementType.Game,
                Evaluate = (player, game, round) =>
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    if (game.Players.Count < 5) return false;
                    return !game.Rounds.SelectMany(x => x.Actions).OfType<LiarAction>().Any(x => x.PlayerId == player.Id);
                }
            },
            new AchievementCheck
            {
                Name = "The Survivalist",
                Description = "Survive 10 or more rounds at one life",
                Type = (int)AchievementType.Game,
                Evaluate = (player, game, round) =>
                {
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    var playerHands = game.Rounds.SelectMany(x => x.PlayerHands).Where(x => x.PlayerId == player.Id).ToList();
                    return playerHands.Count(x => x.Dice.ToIntegerDice().Count == 5) >= 10;
                }
            },
            new AchievementCheck
            {
                Name = "A Sea of Red",
                Description = "Win a game with 5 or more players calling liar every turn",
                Type = (int)AchievementType.Game,
                Evaluate = (player, game, round) =>
                {
                    return false;
                }
            }
        };
    }


}
