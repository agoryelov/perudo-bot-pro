using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public class Rattle
    {
        public int Id { get; set; }
        public int RattleType { get; set; }
        public int RattleContentType { get; set; }
        public string Content { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
