using System.ComponentModel.DataAnnotations.Schema;

namespace PerudoBot.Database.Data
{
    public class ItemDrop : TrackedEntity
    {
        public int Id { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        public int GameId { get; set; }

        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }
        public int ItemId { get; set; }
    }
}
