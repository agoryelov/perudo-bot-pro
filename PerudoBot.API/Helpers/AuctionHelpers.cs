using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Constants;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Helpers
{
    public static class AuctionHelpers
    {
        public static bool IsValidFollowUp(this AuctionBid previousBid, int bidAmount, int itemPrice)
        {
            if (bidAmount < itemPrice) return false;
            if (previousBid == null) return true;

            return bidAmount > previousBid.BidAmount;
        }

        public static int HighestBidAmount(this Auction auction)
        {
            var lastBid = auction.HighestBid;
            if (lastBid == null)
            {
                return auction.AuctionItem.AuctionPrice();
            }

            return lastBid.BidAmount;
        }

        public static int AuctionPrice(this Item item)
        {
            if (item.Price.HasValue)
            {
                return item.Price.Value;
            }

            return ((ItemTier)item.Tier).DefaultPrice();
        }

        public static DiceItem GetDailyAuctionItem(this IQueryable<DiceItem> items)
        {
            items = items.Where(x => x.DropEnabled);

            var rng = DailySeededRandom();
            var tierRoll = rng.Next(100);

            if (tierRoll > 90) items = items.Where(x => x.Tier == (int)ItemTier.Epic);
            else if (tierRoll > 50) items = items.Where(x => x.Tier == (int)ItemTier.Rare);
            else items = items.Where(x => x.Tier == (int)ItemTier.Common);

            var itemList = items.ToList();
            var itemRoll = rng.Next(itemList.Count);
            return itemList[itemRoll];
        }

        private static Random DailySeededRandom()
        {
            var date = DateTime.UtcNow.ToPST();
            var seed = date.Year * 1000 + date.DayOfYear;
            return new Random(seed);
        }

        public static DateTime ToPST(this DateTime dateTime)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
    }
}
