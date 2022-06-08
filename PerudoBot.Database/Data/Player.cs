using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public class Player
    {
        public int Id { get; set; }
        public int Lives { get; set; }
        public int RoundEliminated { get; set; }
        public int TurnOrder { get; set; }
        public ICollection<PlayerHand> PlayerHands { get; set; }
        public ICollection<Round> Rounds { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        public int GameId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
