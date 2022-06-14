using PerudoBot.API.Constants;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Helpers
{
    public static class GameHelpers
    {
        private static Random _random = new Random();

        public static PlayerHand DealHand(this Player player, Round round)
        {
            var numDice = player.Lives;

            if (round.RoundType == (int) RoundType.Reverse)
            {
                numDice = GameConstants.STARTING_LIVES - player.Lives + 1;
            }

            return new PlayerHand
            {
                PlayerId = player.Id,
                RoundId = round.Id,
                Dice = DiceHelpers.GetRandomDice(numDice).ToStringDice()
            };
        }

        public static List<int> GetAllDice(this ICollection<PlayerHand> roundPlayers)
        {
            return roundPlayers
                .SelectMany(x => x.Dice.ToIntegerDice())
                .ToList();
        }

        public static int GetLivesLost(this Round round, int numberOfDiceOffBy)
        {
            if (round.RoundType == (int) RoundType.SuddenDeath)
            {
                return GameConstants.STARTING_LIVES;
            }

            return numberOfDiceOffBy;
        }
        
        public static int GetActualQuantity(this BidAction bid, List<int> allDice)
        {
            return allDice.Count(x => x == bid.Pips || x == 1);
        }

        public static int GetStartingPlayerId(this ICollection<Player> players)
        {
            return players.OrderBy(x => x.TurnOrder).First().Id;
        }

        public static int GetStartingPlayerId(this ICollection<Player> players, int losingPlayerId)
        {
            var losingPlayer = players.Single(x => x.Id == losingPlayerId);

            if (losingPlayer.Lives > 0)
            {
                return losingPlayerId;
            }
            else
            {
                return players.GetNextActivePlayerId(losingPlayerId);
            }
        }

        public static int GetNextActivePlayerId(this ICollection<Player> players, int currentPlayerId)
        {
            var playerIds = players
                .Where(x => x.Lives > 0 || x.Id == currentPlayerId) // in case the current user is eliminated and won't show up
                .OrderBy(x => x.TurnOrder)
                .Select(x => x.Id)
                .ToList();

            var playerIndex = playerIds.FindIndex(x => x == currentPlayerId);

            if (playerIndex >= playerIds.Count - 1)
            {
                return playerIds[0];
            }
            else
            {
                return playerIds[playerIndex + 1];
            }
        }

        public static bool IsValidFollowUp(this BidAction previousBid, int quantity, int pips)
        {
            if (pips > 6 || pips < 1) return false;

            if (previousBid == null && pips == 1) return false;
            if (previousBid == null) return true;

            var previousQuantity = previousBid.Pips == 1 ? previousBid.Quantity * 2 : previousBid.Quantity;
            var followUpQuantity = pips == 1 ? quantity * 2 : quantity;

            if (previousQuantity > followUpQuantity) return false;
            if (previousQuantity == followUpQuantity && previousBid.Pips >= pips) return false;
            return true;
        }

        public static ICollection<Player> Shuffle(this ICollection<Player> players)
        {
            var shuffledPlayers = players.OrderBy(x => _random.Next()).ToList();
            var turnOrder = 0;
            foreach (var gamePlayer in shuffledPlayers)
            {
                gamePlayer.TurnOrder = turnOrder;
                turnOrder += 1;
            }

            return shuffledPlayers;
        }
    }
}
