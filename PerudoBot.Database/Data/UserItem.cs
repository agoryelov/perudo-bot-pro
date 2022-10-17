namespace PerudoBot.Database.Data
{
    public class UserItem : TrackedEntity
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Item Item { get; set; }
        public int ItemId { get; set; }
    }
}
