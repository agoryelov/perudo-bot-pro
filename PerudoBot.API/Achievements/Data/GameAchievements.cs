using PerudoBot.API.Constants;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Achievements
{
    public static partial class AchievementData
    {
        public static List<AchievementCheck> GameAchievements = new List<AchievementCheck>
        {
            new AchievementCheck
            {
                Name = "Close Call",
                Description = "Win a reverse game with 1 life remaining",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Easy,
                Evaluate = (player, game, round) =>
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    return game.ActivePlayers.First().Lives == 1;
                }
            },
            new AchievementCheck
            {
                Name = "Invisible",
                Description = "Survive more than 10 rounds with one die",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Easy,
                Evaluate = (player, game, round) =>
                {
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    var playerHands = game.Rounds.SelectMany(x => x.PlayerHands).Where(x => x.PlayerId == player.Id).ToList();
                    return playerHands.Count(x => x.Dice.ToIntegerDice().Count == 1) > 10;
                }
            },
            new AchievementCheck
            {
                Name = "Chasing Risk",
                Description = "Correclty call liar out of turn 3 times in a game",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    return game.Rounds.SelectMany(x => x.Actions).OfType<LiarAction>().Where(x => x.OutOfTurn).Count() >= 3;
                }
            },
            new AchievementCheck
            {
                Name = "Hat Trick",
                Description = "Win 3 games in a row",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Medium,
                Evaluate = (player, game, round) =>
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    var recentGames = player.User.Games.TakeLast(3);
                    if (recentGames.Count() < 3) return false;
                    return recentGames.All(x => x.WinningPlayerId == player.Id);
                }
            },
            new AchievementCheck
            {
                Name = "Not a Scratch",
                Description = "Win a reverse game with 5 or more players without taking damage",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Hard,
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
                Score = (int)AchievementScore.Hard,
                Evaluate = (player, game, round) =>
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    if (game.Players.Count < 5) return false;
                    return !game.Rounds.SelectMany(x => x.Actions).OfType<LiarAction>().Any(x => x.PlayerId == player.Id);
                }
            },
            new AchievementCheck
            {
                Name = "Not Today",
                Description = "Survive 8 or more rounds at one life",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Hard,
                Evaluate = (player, game, round) =>
                {
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    var playerHands = game.Rounds.SelectMany(x => x.PlayerHands).Where(x => x.PlayerId == player.Id).ToList();
                    return playerHands.Count(x => x.Dice.ToIntegerDice().Count == 5) >= 8;
                }
            },
            new AchievementCheck
            {
                Name = "A Sea of Red",
                Description = "Win a game with 5 or more players calling liar every round",
                Type = (int)AchievementType.Game,
                Score = (int)AchievementScore.Hard,
                Evaluate = (player, game, round) =>
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    if (game.Players.Count < 5) return false;
                    return game.Rounds.SelectMany(x => x.Actions).OfType<LiarAction>().All(x => x.PlayerId == player.Id);
                }
            }
        };
    }
}
