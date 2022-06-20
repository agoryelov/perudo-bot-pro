using PerudoBot.API.Achievements;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Helpers
{
    public static class MapHelpers
    {
        public static PlayerDto ToPlayerDto(this Player player, int? roundId = null)
        {  
            var playerState = new PlayerDto
            {
                PlayerId = player.Id,
                DiscordId = player.User.DiscordId,
                IsBot = player.User.IsBot,
                Points = player.User.Points,
                Name = player.User.Name,
                Lives = player.Lives,
            };

            if (roundId == null) return playerState;

            var playerHand = player.PlayerHands.SingleOrDefault(x => x.RoundId == roundId);

            if (playerHand == null) return playerState;

            playerState.Dice = playerHand.Dice.ToIntegerDice();
            return playerState;
        }

        public static RoundDto ToRoundDto(this Round round)
        {
            var roundState = new RoundDto
            {
                ActivePlayerId = round.ActivePlayerId,
                RoundNumber = round.RoundNumber,
                RoundType = round.RoundType,
                Bets = round.Actions.OfType<BetAction>().Select(x => x.ToBetDto()).ToList(),
                Bids = round.Actions.OfType<BidAction>().Select(x => x.ToBidDto()).ToList(),
                LatestBid = round.LatestBid?.ToBidDto(),
                Liar = round.Liar?.ToLiarDto(),
                Players = round.Game.Players.OrderBy(x => x.TurnOrder).Select(x => x.ToPlayerDto(round.Id)).ToList()
            };
            roundState.TotalDiceCount = roundState.Players.Sum(x => x.Dice.Count);
            roundState.ActivePlayerCount = roundState.Players.Count(x => x.Lives > 0);
            return roundState;
        }

        public static GameSetupDto ToGameSetupDto(this Game game)
        {
            return new GameSetupDto
            {
                GameId = game.Id,
                Players = game.Players.Select(x => x.ToPlayerDto()).ToList(),
                DefaultRoundType = game.DefaultRoundType
            };
        }

        public static BidDto ToBidDto(this BidAction bid)
        {
            if (bid == null) return null;
            return new BidDto
            {
                ActionType = bid.ActionType,
                Pips = bid.Pips,
                Quantity = bid.Quantity,
                PlayerId = bid.PlayerId,
                DateCreated = bid.DateCreated.ToString("yyyy-MM-ddTHH:mm:sszzz")
            };
        }

        public static LiarDto ToLiarDto(this LiarAction liar)
        {
            if (liar == null) return null;
            return new LiarDto
            {
                PlayerId = liar.PlayerId,
                ActionType = liar.ActionType,
                TargetBid = liar.TargetBid.ToBidDto(),
                IsSuccessful = liar.IsSuccessful,
                WinningPlayerId = liar.IsSuccessful ? liar.PlayerId : liar.TargetBid.PlayerId,
                LosingPlayerId = liar.LosingPlayerId,
                LivesLost = liar.LivesLost,
                ActualQuantity = liar.ActualQuantity
            };
        }

        public static BetDto ToBetDto(this BetAction bet)
        {
            if (bet == null) return null;
            return new BetDto
            {
                PlayerId = bet.PlayerId,
                ActionType = bet.ActionType,
                TargetBid = bet.TargetBid.ToBidDto(),
                IsSuccessful = bet.IsSuccessful,
                BetAmount = bet.BetAmount,
                BetOdds = bet.BetOdds,
                BetType = bet.BetType
            };
        }
        
        public static PlayerPointsChange ToPlayerPointsChange(this Player player, int pointChange)
        {
            return new PlayerPointsChange
            {
                Name = player.User.Name,
                StartingPoints = player.User.Points - pointChange,
                FinalPoints = player.User.Points
            };
        }

        public static PlayerEloChange ToPlayerEloChange(this Player player, int eloChange)
        {
            return new PlayerEloChange
            {
                Name = player.User.Name,
                StartingElo = player.User.Elo - eloChange,
                FinalElo = player.User.Elo
            };
        }

        public static GameNote ToGameNote(this RoundNote note)
        {
            return new GameNote
            {
                RoundNumber = note.RoundNumber,
                Name = note.User.Name,
                Text = note.Text
            };
        }

        public static Achievement ToAchievement(this AchievementCheck achievement)
        {
            return new Achievement
            {
                Name = achievement.Name,
                Description = achievement.Description,
                Type = achievement.Type
            };
        }

        public static AchievementDto ToAchievementDto(this Achievement achievement)
        {
            return new AchievementDto
            {
                Name = achievement.Name,
                Description = achievement.Description,
                UnlocksCount = achievement.UserAchievements.Count(),
                UnlockedBy = achievement.UserAchievements.FirstOrDefault()?.User.Name,
                DateUnlocked = achievement.UserAchievements.FirstOrDefault()?.DateCreated.ToString("yyyy-MM-ddTHH:mm:sszzz")
            };
        }

        public static UserAchievementDto ToUserAchievementDto(this UserAchievement achievement)
        {
            return new UserAchievementDto
            {
                DateUnlocked = achievement.DateCreated.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                Description = achievement.Achievement.Description,
                Name = achievement.Achievement.Name,
                Username = achievement.User.Name
            };
        }

        public static LadderEntryDto ToLadderEntryDto(this User user)
        {
            return new LadderEntryDto
            {
                Name = user.Name,
                Elo = user.Elo,
                Points = user.Points,
                GamesPlayed = user.Players.Count(x => x.Game.State == (int)GameState.Ended)
            };
        }
    }
}
