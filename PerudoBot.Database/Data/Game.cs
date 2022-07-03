using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public class Game : TrackedEntity
    {
        public int Id { get; set; }
        public int State { get; set; }
        public int DefaultRoundType { get; set; }

        public ICollection<Round> Rounds { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<UserLog> UserLogs { get; set; }

        [ForeignKey("SeasonId")]
        public Season Season { get; set; }
        public int SeasonId { get; set; }

        public int WinningPlayerId { get; set; }

        [NotMapped]
        public Round LatestRound => Rounds.LastOrDefault();

        [NotMapped]
        public ICollection<Player> ActivePlayers => Players.Where(x => x.Lives > 0).ToList();

        [NotMapped]
        public ICollection<Player> EliminatedPlayers => Players.Where(x => x.Lives <= 0).ToList();
    }
}
