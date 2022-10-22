using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PerudoBot.Database.Data
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public ICollection<UserItem> UserItems { get; set; }
        public int Tier { get; set; }
        public int? Price { get; set; }
        public bool DropEnabled { get; set; }
        public string Content { get; set; }
    }

    public class DiceItem : Item
    {
    }
}
