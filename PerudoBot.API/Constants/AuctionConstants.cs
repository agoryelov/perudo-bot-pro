namespace PerudoBot.API.Constants
{
    public static class AuctionConstants
    {
        public static int DefaultPrice(this ItemTier itemTier)
        {
            switch (itemTier)
            {
                case ItemTier.Common: return 2000;
                case ItemTier.Rare: return 4000;
                case ItemTier.Epic: return 8000;
                default: throw new ArgumentOutOfRangeException("itemTier");
            }
        }
    }

    public enum ItemTier
    {
        Common = 0,
        Rare = 1,
        Epic = 2
    }

    public enum AuctionState
    {
        Setup = 0,
        InProgress = 1,
        Terminated = 2,
        Ended = 3
    }
}
