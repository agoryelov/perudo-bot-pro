﻿using PerudoBot.API.Constants;
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

        public RoundDto Bet(Game game, Player player, int betAmount, BetType betType, int targetBidId)
        {
            if (player.User.Points <= 0)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You have no points" };
            }

            if (player.User.Points < betAmount)
            {
                betAmount = player.User.Points;
            }

            if (game.LatestRound?.LatestBid == null)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "There is no bid to bet on" };
            }

            var currentRound = game.LatestRound;
            var targetBid = _db.BidActions.SingleOrDefault(x => x.Id == targetBidId && x.RoundId == currentRound.Id);

            if (targetBid == null)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can't bet on this bid" };
            }

            if (targetBid.PlayerId == player.Id)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can't bet on your own bid" };
            }

            
            var allowedBetAmount = MaxBetAmount(currentRound, betType);

            if (allowedBetAmount <= 0)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can't bet that much right now" };
            }

            if (allowedBetAmount < betAmount)
            {
                betAmount = allowedBetAmount;
            }

            var existingBet = currentRound.Actions.OfType<BetAction>().FirstOrDefault(x => x.PlayerId == player.Id);

            if (existingBet == null)
            {
                _userService.RemoveBetPoints(player.User, betAmount, game);

                var betAction = BetOnAction(currentRound, player, betAmount, betType, targetBid);
                currentRound.Actions.Add(betAction);

                _db.SaveChanges();
                return currentRound.ToRoundDto();
            }

            if (existingBet.TargetBidId != targetBid.Id)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can only bet once per round" };
            }

            if (existingBet.BetType != (int)betType)
            {
                return new RoundDto { RequestSuccess = false, ErrorMessage = "You can't change your bet type" };
            }

            _userService.RemoveBetPoints(player.User, betAmount, game);
            existingBet.BetAmount += betAmount;

            _db.SaveChanges();
            return currentRound.ToRoundDto();
        }

        //private void LowerOddsFromPreviousBets(Round round, BetAction bet)
        //{
        //    if (bet.BetType == (int)BetType.Legit) return;
        //    var prevBets = round.Actions.OfType<BetAction>().Count(x => x.TargetBidId == round.LatestBid.Id && x.BetType == bet.BetType);
        //    var oddsBefore = bet.BetOdds;
        //    if (prevBets > 0) bet.BetOdds = 1 + ((bet.BetOdds - 1) * 0.5);
        //}

        private BetAction BetOnAction(Round currentRound, Player player, int betAmount, BetType betType, BidAction action)
        {
            var playerHand = currentRound.PlayerHands.SingleOrDefault(x => x.PlayerId == player.Id);

            var playerDice = new List<int>();
            if (playerHand != null) playerDice = playerHand.Dice.ToIntegerDice();

            var gameDice = currentRound.PlayerHands.GetAllDice();

            var bet = new BetAction
            {
                PlayerId = player.Id,
                ParentActionId = currentRound.LatestAction?.Id,
                RoundId = currentRound.Id,
                TargetBid = action,
                BetAmount = betAmount,
                BetType = (int)betType
            };

            return bet.SetOutcome(gameDice, playerDice);
        }

        public Response AwardBetPoints(Round round)
        {
            if (round == null)
            {
                return Responses.Error("Can't resolve bets for current round");
            }

            RefundLegitOverflow(round);

            var bets = round.Actions.OfType<BetAction>().Where(x => x.IsSuccessful);
            foreach (var bet in bets)
            {
                _userService.AddBetPoints(bet.Player.User, bet.WinAmount(), round.Game);
            }

            return Responses.OK();
        }

        private void RefundLegitOverflow(Round round)
        {
            var legitBets = round.Actions.OfType<BetAction>().Where(x => x.BetType == (int)BetType.Legit);
            foreach (var bet in legitBets)
            {
                var maxLegitAmount = MaxLegitAmount(round, bet);
                if (bet.BetAmount > maxLegitAmount)
                {
                    var overflowPoints = bet.BetAmount - maxLegitAmount;
                    _userService.AddBetPoints(bet.Player.User, overflowPoints, round.Game);
                    bet.BetAmount = maxLegitAmount;
                }
            }
        }

        private int MaxBetAmount(Round round, BetType betType)
        {
            var existingBets = round.Actions.OfType<BetAction>()
                .Where(x => x.BetType == (int)betType)
                .Where(x => x.TargetBid == round.LatestBid)
                .ToList().Sum(x => x.BetAmount);
            var maxBetAmount = betType.MaxBetPerDie() * round.PlayerHands.GetAllDice().Count;
            return maxBetAmount - existingBets;
        }

        private int MaxLegitAmount(Round round, BetAction bet)
        {
            var numLegitBets = (double)round.Actions.OfType<BetAction>()
                    .Where(x => x.BetType == (int)BetType.Legit)
                    .Where(x => x.TargetBidId == bet.TargetBidId)
                    .ToList().Count;
            var sumLiarBets = (double)round.Actions.OfType<BetAction>()
                    .Where(x => x.PlayerId != bet.PlayerId)
                    .Where(x => x.BetType == (int)BetType.Liar)
                    .Where(x => x.TargetBidId == bet.TargetBidId)
                    .ToList().Sum(x => x.BetAmount);
            return (int)Math.Round(sumLiarBets / numLegitBets);
        }
    }
}
