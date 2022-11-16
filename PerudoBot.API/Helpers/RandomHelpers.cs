using PerudoBot.Database.Data;

namespace PerudoBot.API.Helpers
{
    public static class RandomHelpers
    {
        private static Random _random = new Random();

        public static Player RandomPlayerWeighted(this Game game)
        {
            var weightedPool = new List<Player>();

            foreach (var player in game.Players)
            {
                for (var i = 0; i < player.RoundEliminated; i++) weightedPool.Add(player);
            }

            var poolIndex = _random.Next(weightedPool.Count);
            return weightedPool[poolIndex];
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

        public static int PickItemWeighted((int, int)[] itemProbs)
        {
            var probSum = 0;
            foreach (var itemProb in itemProbs)
            {
                var probability = itemProb.Item2;
                probSum += probability;
            }

            var roll = _random.Next(probSum);

            var probCounter = 0;
            foreach (var itemProb in itemProbs)
            {
                var probability = itemProb.Item2;

                if (probCounter + probability >= roll)
                {
                    var item = itemProb.Item1;
                    return item;
                }
                probCounter += probability;
            }

            return 0;
        }
    }
}
