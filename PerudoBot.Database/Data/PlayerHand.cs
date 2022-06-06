using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public class PlayerHand
    {
        public int Id { get; set; }
        public string Dice { get; set; }

        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey("RoundId")]
        public Round Round { get; set; }
        public int RoundId { get; set; }
    }
}
