using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public class RoundNote
    {
        public int Id { get; set; }

        [ForeignKey("RoundId")]
        public Round Round { get; set; }
        public int RoundId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }

        public string Text  { get; set; }

        [NotMapped]
        public int RoundNumber => Round.RoundNumber; 
    }
}
