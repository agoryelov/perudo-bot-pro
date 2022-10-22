namespace PerudoBot.API.DTOs
{
    public class UserInventoryDto : Response
    {
        public string Name { get; set; }
        public ItemDto EquippedDice { get; set; }
        public List<ItemDto> DiceItems { get; set; }
    }

    public class ItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Price { get; set; }
        public string Content { get; set; }
    }
}
