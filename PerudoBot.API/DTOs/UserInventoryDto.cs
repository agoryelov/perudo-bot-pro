namespace PerudoBot.API.DTOs
{
    public class UserInventoryDto : Response
    {
        public string Name { get; set; }
        public DiceItemDto EquippedDice { get; set; }
        public List<DiceItemDto> DiceItems { get; set; }
    }

    public class UserItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
    }

    public class DiceItemDto : UserItemDto
    {
        public string DiceEmotes { get; set; }
    }
}
