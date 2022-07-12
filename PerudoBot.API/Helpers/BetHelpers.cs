using PerudoBot.API.Constants;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Helpers
{
    public static class BetHelpers
    {
        public static int WinAmount(this BetAction bet)
        {
            if (!bet.IsSuccessful) return 0;
            return (int)Math.Round(bet.BetAmount * bet.BetOdds);
        }

        public static BetAction SetOutcome(this BetAction bet, List<int> allDice, List<int> playerDice)
        {
            if (bet.BetType == (int)BetType.Liar)
            {
                bet.IsSuccessful = bet.IsLiar(allDice);
                bet.BetOdds = GameConstants.LIAR_ODDS;
            }

            if (bet.BetType == (int)BetType.Exact)
            {
                bet.IsSuccessful = bet.IsExact(allDice);
                bet.BetOdds = GameConstants.EXACT_ODDS;
            }

            if (bet.BetType == (int)BetType.Peak)
            {
                bet.IsSuccessful = bet.IsPeak(allDice);
                bet.BetOdds = GameConstants.PEAK_ODDS;
            }

            if (bet.BetType == (int)BetType.Legit)
            {
                bet.IsSuccessful = bet.IsLegit(allDice);
                bet.BetOdds = GameConstants.LEGIT_ODDS;
            }

            if (bet.IsGuaranteed(playerDice, allDice.Count)) bet.BetOdds = GameConstants.PITY_ODDS;

            return bet;
        }

        private static bool IsGuaranteed(this BetAction bet, List<int> playerDice, int totalDice)
        {
            if (bet.BetType == (int)BetType.Exact) return false;
            if (bet.BetType == (int)BetType.Peak) return false;

            var playerQuantity = playerDice.Count(x => x == bet.BetPips || x == 1);

            if (bet.BetType == (int)BetType.Liar) return bet.BetQuantity > (totalDice + playerQuantity);
            if (bet.BetType == (int)BetType.Legit) return bet.BetQuantity <= playerQuantity;

            return false;
        }

        private static bool IsExact(this BetAction bet, List<int> allDice)
        {
            return bet.BetQuantity == allDice.Count(x => x == bet.BetPips || x == 1);
        }

        private static bool IsLiar(this BetAction bet, List<int> allDice)
        {
            return bet.BetQuantity > allDice.Count(x => x == bet.BetPips || x == 1);
        }

        private static bool IsPeak(this BetAction bet, List<int> allDice)
        {
            if (!bet.IsExact(allDice)) return false;

            var bidIndex = BidToActionIndex(bet.BetPips, bet.BetQuantity, allDice.Count);
            for (int face = 1; face <= 6; face++)
            {
                var faceQuantity = allDice.Count(x => x == face || x == 1);
                var faceIndex = BidToActionIndex(face, faceQuantity, allDice.Count);
                if (faceIndex > bidIndex) return false;
            }

            return true;
        }

        private static bool IsLegit(this BetAction bet, List<int> allDice)
        {
            return !bet.IsLiar(allDice);
        }

        private static int BidToActionIndex(int pips, int quantity, int total)
        {
            if (pips != 1)
            {
                var wildcard = quantity / 2;
                var non_wildcard = (quantity - 1) * 5;
                return wildcard + non_wildcard + (pips - 2);
            }
            else
            {
                var abs_index = 5 + ((quantity - 1) * 11);
                var adjust = Math.Max(quantity - 1 - (total - quantity), 0) * 5;
                return abs_index - adjust;
            }
        }
    }
}
