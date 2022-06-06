using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerudoBot.Database.Data
{
    public class UserAchievement : TrackedEntity
    {
        public int Id { get; set; }
        public bool IsNew { get; set; } = true;

        [ForeignKey("AchievementId")]
        public Achievement Achievement { get; set; }
        public int AchievementId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
