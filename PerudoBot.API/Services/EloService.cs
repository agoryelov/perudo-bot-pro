using PerudoBot.Database.Data;

namespace PerudoBot.API.Services
{
    public class EloService
    {
        private readonly UserService _userService;

        public EloService(UserService userService)
        {
            _userService = userService;
        }

        private class UserEloChange
        {
            public User User { get; set; }
            public int StartingElo { get; set; }
            public double EloChange { get; set; }
        }

        private double CalculateExpectedScore(int playerElo, int opponentElo)
        {
            return 1 / (1.0f + (float)Math.Pow(10.0f, (opponentElo - playerElo) / 400.0f));
        }

        // Source: https://github.com/FigBug/Multiplayer-ELO/blob/master/csharp/elo.cs
        public void UpdateElo(Game completedGame, double startingK = 10)
        {
            var playerRanking = completedGame.Players
                .OrderByDescending(p => p.RoundEliminated)
                .Select(p => new UserEloChange
                {
                    User = p.User,
                    StartingElo = p.User.Elo,
                    EloChange = 0
                }).
                ToList();

            var playerCount = playerRanking.Count;
            var K = startingK;

            if (playerCount > 5)
            {
                K = startingK * (float)(5.0 / playerCount);
            }

            for (int i = 0; i < playerCount; i++)
            {
                var player = playerRanking[i];

                // Calculate elo changes compared to players above
                for (int j = 0; j < i; j++)
                {
                    var playerAbove = playerRanking[j];
                    var expectedScore = CalculateExpectedScore(player.StartingElo, playerAbove.StartingElo);
                    player.EloChange += K * (0.0f - expectedScore);
                }


                // Calculate elo changes compared to player below
                for (int j = i + 1; j < playerCount; j++)
                {
                    var playerBelow = playerRanking[j];
                    var expectedScore = CalculateExpectedScore(player.StartingElo, playerBelow.StartingElo);
                    player.EloChange += K * (1.0f - expectedScore);
                }

                var updatedElo = player.StartingElo + (int)Math.Round(player.EloChange);

                 _userService.UpdateElo(player.User, updatedElo, completedGame);
            }
        }
    }

    
}
