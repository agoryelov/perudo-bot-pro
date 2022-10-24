using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public abstract class TrackedEntity
    {
        public DateTime DateCreated { get; set; }
        public TrackedEntity()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}
