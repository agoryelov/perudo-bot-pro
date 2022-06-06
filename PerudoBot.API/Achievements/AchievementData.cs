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
                Name = "Enforcer",
                Description = "Call a quick maths six bid at the start of the round",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (round.Actions.OfType<BidAction>().Count() != 1) return false;
                    var firstBid = round.Actions.OfType<BidAction>().First();
                    if (firstBid.Pips != 6) return false;
                    var quickMaths = (int) Math.Round(round.PlayerHands.GetAllDice().Count / 3.0);
                    return firstBid.Quantity == quickMaths;
                }
            },
            new AchievementCheck
            {
                Name = "Sniper",
                Description = "Call liar out of turn",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    if (round.Liar.PlayerId != player.Id) return false;
                    if (round.ActivePlayerId == player.Id) return false;
                    else return true;
                }
            },
            //new AchievementCheck
            //{
            //    Name = "Executioner",
            //    Description = "Eliminate a player with a liar call",
            //    Type = (int)AchievementType.Round,
            //    Evaluate = (player, game, round) =>
            //    {
            //        if (round.Liar.PlayerId != player.Id) return false;
            //        if (round.Liar.LosingPlayerId == player.Id) return false;
            //        var losingPlayer = round.Players.First(x => x.Id == round.Liar.LosingPlayerId);
            //        return (losingPlayer.Lives == 0);
            //    }
            //},
            new AchievementCheck
            {
                Name = "High Roller",
                Description = "Go all in on exact a bet and win",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    if (player.User.Points != 0) return false;
                    var exactBet = round.Actions.OfType<BetAction>()
                        .FirstOrDefault(x => x.PlayerId == player.Id && x.BetType == (int)BetType.Exact);
                    if (exactBet == null) return false;
                    return exactBet.IsSuccessful;
                }
            },
            new AchievementCheck
            {
                Name = "Pants on Fire",
                Description = "Place a bid on a number you don't have",
                Type = (int)AchievementType.Round,
                Evaluate = (player, game, round) =>
                {
                    var playerHand = round.PlayerHands.SingleOrDefault(x => x.PlayerId == player.Id);
                    if (playerHand == null) return false;
                    var playerDice = playerHand.Dice.ToIntegerDice();
                    if (playerDice.Contains(1)) return false;
                    return round.Actions.OfType<BidAction>().Any(x => x.PlayerId == player.Id && !playerDice.Contains(x.Pips));
                }
            }
        };

        public static List<AchievementCheck> GameAchievements = new List<AchievementCheck>
        {
            new AchievementCheck
            {
                Name = "Survivalist",
                Description = "Win a Reverse Mode game with 1 life remaining",
                Type = (int)AchievementType.Game,
                Evaluate = (player, game, round) => 
                {
                    if (game.WinningPlayerId != player.Id) return false;
                    if (game.DefaultRoundType != (int)RoundType.Reverse) return false;
                    if (game.ActivePlayers.First().Lives != 1) return false;
                    else return true;
                }
            },
            new AchievementCheck
            {
                Name = "Chicken Dinner",
                Description = "Win a game of Perudo",
                Type = (int)AchievementType.Game,
                Evaluate = (player, game, round) =>
                {
                    return game.WinningPlayerId == player.Id;
                }
            },
        };
    }


}
