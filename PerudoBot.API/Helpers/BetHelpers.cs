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

        public static double BidChance(int pips, int quantity, int total, int face_count = 6)
        {
            if (quantity < 0) return 0;
            if (quantity > total) return 0;

            var prob = pips == 1 ? (1.0 / face_count) : (2.0 / face_count);
            var chance1 = Math.Pow(prob, quantity);
            var chance2 = Math.Pow(1 - prob, total - quantity);
            var permute = Factorial(total) / (Factorial(quantity) * Factorial(total - quantity));
            return chance1 * chance2 * permute;
        }

        public static double BidChanceOrMore(int pips, int quantity, int total, int face_count = 6)
        {
            var sum = 0.0;
            for (var i = quantity; i <= total; i++)
            {
                sum += BidChance(pips, i, total, face_count);
            }

            if (sum < 0) return 0;
            if (sum > 1) return 1;
            return sum;
        }

        public static double Factorial(int num)
        {
            if (num == 0)
                return 1;
            else
                return num * Factorial(num - 1);
        }
    }
}
