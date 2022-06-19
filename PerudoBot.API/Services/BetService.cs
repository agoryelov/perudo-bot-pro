using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Services
{
    public class BetService
    {
        private readonly PerudoBotDbContext _db;
        private readonly UserService _userService;

        public BetService(PerudoBotDbContext context, UserService userService)
        {
            _db = context;
            _userService = userService;
        }

        public RoundDto Bet(Game game, Player player, int betAmount, BetType betType)
        {
            if (player.User.Points < betAmount)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "Insufficient points to place this bet" };
            }

            if (game.LatestRound?.LatestBid == null)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "There is no bid to bet on" };
            }

            var currentRound = game.LatestRound;

            if (currentRound.IsCompleted)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "Round is completed" };
            }

            if (currentRound.LatestBid.PlayerId == player.Id)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can't bet on your own bid" };
            }

            var existingBet = currentRound.Actions.OfType<BetAction>().FirstOrDefault(x => x.PlayerId == player.Id);

            if (existingBet == null)
            {
                _userService.RemoveBetPoints(player.User, betAmount, game);
                currentRound.Actions.Add(BetOnLatestAction(currentRound, player, betAmount, betType));

                _db.SaveChanges();
                return currentRound.ToRoundDto();
            }

            if (existingBet.TargetBidId != currentRound.LatestBid.Id)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can only bet once per round" };
            }

            if (existingBet.BetType != (int) betType)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can't change your bet type" };
            }

            _userService.RemoveBetPoints(player.User, betAmount, game);
            existingBet.BetAmount += betAmount;

            _db.SaveChanges();
            return currentRound.ToRoundDto();
        }

        public Response AwardBetPoints(Round round)
        {
            if (round == null)
            {
                return Responses.Error("Can't resolve bets for current round");
            }

            var bets = round.Actions.OfType<BetAction>().Where(x => x.IsSuccessful);

            foreach(var bet in bets)
            {
                var pointsGained = (int)Math.Round(bet.BetAmount * bet.BetOdds);
                _userService.AddBetPoints(bet.Player.User, pointsGained, round.Game);
            }

            return Responses.OK();
        }

        private BetAction BetOnLatestAction(Round currentRound, Player player, int betAmount, BetType betType)
        {
            var currentBid = currentRound.LatestBid;
            var gameDice = currentRound.PlayerHands.GetAllDice();

            var targetPips = currentBid.Pips;
            var targetQuantity = currentBid.Quantity;

            var actualQuantity = gameDice.Where(x => x == targetPips || x == 1).Count();

            var bet = new BetAction
            {
                PlayerId = player.Id,
                ParentActionId = currentRound.LatestAction?.Id,
                TargetBidId = currentBid.Id,
                RoundId = currentRound.Id,
                BetAmount = betAmount,
                BetType = (int) betType
            };

            if (bet.BetType == (int)BetType.Liar)
            {
                bet.IsSuccessful = actualQuantity < targetQuantity;
                bet.BetOdds = 1.0 / (1 - BetHelpers.BidChanceOrMore(targetPips, targetQuantity, gameDice.Count));
            }

            if (bet.BetType == (int)BetType.Exact)
            {
                bet.IsSuccessful = actualQuantity == targetQuantity;
                bet.BetOdds = 1.0 / BetHelpers.BidChance(targetPips, targetQuantity, gameDice.Count);
            }

            bet.BetOdds = Math.Max(Math.Min(bet.BetOdds, GameConstants.MAX_BET_ODDS), GameConstants.MIN_BET_ODDS);

            return bet;
        }
    }
}
