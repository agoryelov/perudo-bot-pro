using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public class User
    {
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public bool IsBot { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Elo { get; set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<Achievement> Achievements { get; set; }
    }
}
