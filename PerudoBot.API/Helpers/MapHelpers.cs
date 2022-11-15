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
                EquippedDice = player.User.EquippedDice?.Content,
                Rattles = player.User.Rattles.Select(x => x.ToRattleDto()).ToList()
            };

            if (roundId == null) return playerState;

            var playerHand = player.PlayerHands.SingleOrDefault(x => x.RoundId == roundId);

            if (playerHand == null) return playerState;

            playerState.Dice = playerHand.Dice.ToIntegerDice();
            return playerState;
        }

        public static RattleDto ToRattleDto(this Rattle rattle)
        {
            return new RattleDto
            {
                RattleType = rattle.RattleType,
                RattleContentType = rattle.RattleContentType,
                Content = rattle.Content
            };
        }

        public static RoundDto ToRoundDto(this Round round)
        {
            var roundState = new RoundDto
            {
                ActivePlayerId = round.ActivePlayerId,
                RoundNumber = round.RoundNumber,
                RoundType = round.RoundType,
                CanReverse = !round.Actions.OfType<ReverseAction>().Any(),
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

        public static GameSetupDto ToGameSetupDto(this Game game, Auction dailyAuction = null)
        {
            return new GameSetupDto
            {
                GameId = game.Id,
                Players = game.Players.Select(x => x.ToPlayerDto()).ToList(),
                DefaultRoundType = game.DefaultRoundType,
                AuctionSetup = dailyAuction?.ToAuctionSetupDto()
            };
        }

        public static BidDto ToBidDto(this BidAction bid)
        {
            if (bid == null) return null;
            return new BidDto
            {
                BidId = bid.Id,
                ActionType = bid.ActionType,
                Pips = bid.Pips,
                Quantity = bid.Quantity,
                PlayerId = bid.PlayerId,
                DateCreated = bid.DateCreated.ToPST().ToString(GameConstants.DATE_FORMAT)
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
                Type = achievement.Type,
                Score = achievement.Score
            };
        }

        public static AchievementDto ToAchievementDto(this Achievement achievement)
        {
            return new AchievementDto
            {
                Name = achievement.Name,
                Description = achievement.Description,
                Score = achievement.Score
            };
        }

        public static AchievementDetailsDto ToAchievementDetailsDto(this Achievement achievement)
        {
            return new AchievementDetailsDto(achievement.ToAchievementDto())
            {
                UnlockedBy = achievement.UserAchievements.OrderBy(x => x.DateCreated).Select(x => x.User?.Name).ToList()
            };
        }

        public static UserAchievementDto ToUserAchievementDto(this UserAchievement achievement)
        {
            return new UserAchievementDto(achievement.Achievement.ToAchievementDto())
            {
                Username = achievement.User?.Name,
                DateUnlocked = achievement.DateCreated.ToString(GameConstants.DATE_FORMAT)
            };
        }

        public static LadderEntryDto ToLadderEntryDto(this User user)
        {
            return new LadderEntryDto
            {
                Name = user.Name,
                Elo = user.Elo,
                Points = user.Points,
                AchievementScore = user.AchievementScore,
                GamesPlayed = user.Games.Count(x => x.State == (int)GameState.Ended)
            };
        }

        public static UserProfileDto ToUserProfileDto(this User user)
        {
            return new UserProfileDto
            {
                Name = user.Name,
                DiscordId = user.DiscordId,
                Elo = user.Elo,
                Points = user.Points,
                Score = user.AchievementScore,
                RecentGames = user.Games?.Completed().TakeLast(GameConstants.RECENT_GAMES).Select(x => x.ToUserGameDto(user)).ToList(),
                RecentAchievements = user.UserAchievements?.OrderByDescending(x => x.DateCreated).Take(GameConstants.RECENT_ACHIEVEMENTS).Select(x => x.ToUserAchievementDto()).ToList()
            };
        }

        public static UserGameDto ToUserGameDto(this Game game, User user)
        {
            var player = game.Players.SingleOrDefault(x => x.UserId == user.Id);

            return new UserGameDto
            {
                Placing = game.Players.OrderByDescending(x => x.RoundEliminated).ToList().IndexOf(player) + 1,
                PlayerCount = game.Players.Count(),
                NetPoints = game.UserLogs.OfType<PointsLog>().Where(x => x.UserId == user.Id).Sum(x => x.PointsChange)
            };
        }

        public static UserInventoryDto ToUserInventoryDto(this User user)
        {
            return new UserInventoryDto
            {
                Name = user.Name,
                EquippedDice = user.EquippedDice?.ToItemDto(),
                DiceItems = user.UserItems.Select(x => x.Item).OfType<DiceItem>().Select(x => x.ToItemDto()).ToList()
            };
        }

        public static ItemDto ToItemDto(this Item item)
        {
            return new ItemDto
            {
                ItemId = item.Id,
                ItemType = item.ItemType,
                ItemName = item.Name,
                Content = item.Content,
                Price = item.AuctionPrice()
            };
        }

        public static AuctionActionDto ToAuctionActionDto(this AuctionAction action)
        {
            var actionDto = new AuctionActionDto 
            { 
                PlayerId = action.AuctionPlayerId,
                DateCreated = action.DateCreated.ToPST().ToString(GameConstants.DATE_FORMAT)
            };

            if (action.ActionType == "AuctionBid") actionDto.BidAmount = ((AuctionBid)action).BidAmount;
            if (action.ActionType == "AuctionPass") actionDto.IsPass = true;

            return actionDto;
        }

        public static AuctionPlayerDto ToAuctionPlayerDto(this AuctionPlayer player)
        {
            return new AuctionPlayerDto
            {
                PlayerId = player.Id,
                Points = player.User.Points,
                Name = player.User.Name,
                IsActive = player.IsActive,
                DiscordId = player.User.DiscordId,
                GamePlayerId = player.GamePlayerId
            };
        }

        public static AuctionDto ToAuctionDto(this Auction auction)
        {
            var auctionDto = new AuctionDto
            {
                AuctionId = auction.Id,
                Actions = auction.AuctionActions.Select(x => x.ToAuctionActionDto()).ToList(),
                Item = auction.AuctionItem.ToItemDto(),
                Players = auction.AuctionPlayers.Select(x => x.ToAuctionPlayerDto()).ToList(),
                HighestBid = auction.HighestBid?.ToAuctionActionDto()
            };

            auctionDto.ActivePlayerCount = auctionDto.Players.Count(x => x.IsActive);
            auctionDto.BidCount = auctionDto.Actions?.Count(x => !x.IsPass) ?? 0;

            return auctionDto;
        }

        public static AuctionSetupDto ToAuctionSetupDto(this Auction auction)
        {
            return new AuctionSetupDto
            {
                AuctionId = auction.Id,
                Item = auction.AuctionItem.ToItemDto()
            };
        }

        public static AuctionSummaryDto ToAuctionSummaryDto(this Auction auction)
        {
            var summary = new AuctionSummaryDto
            {
                Item = auction.AuctionItem.ToItemDto(),
                FinalPrice = auction.HighestBidAmount(),
                Winner = auction.ActivePlayers.SingleOrDefault()?.ToAuctionPlayerDto()
            };

            summary.HasWinner = summary.Winner != null;
            return summary;
        }
    }
}
