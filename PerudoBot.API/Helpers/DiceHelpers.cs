namespace PerudoBot.API.Helpers
{
    public static class DiceHelpers
    {
        private static Random _random = new Random();

        public static List<int> GetRandomDice(int n)
        {
            var dice = new List<int>();
            for (int i = 0; i < n; i++)
            {
                dice.Add(_random.Next(1, 6 + 1));
            }
            dice.Sort();

            return dice;
        }

        public static string ToStringDice(this List<int> dice)
        {
            return string.Join(",", dice);
        }

        public static List<int> ToIntegerDice(this string dice)
        {
            if (string.IsNullOrEmpty(dice)) return new List<int>();
            return dice
                .Split(",")
                .Select(x => int.Parse(x))
                .ToList();
        }
    }
}
