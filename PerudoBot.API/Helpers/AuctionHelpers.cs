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

        public static int AuctionDay(this DateTime date)
        {
            return date.Year * 1000 + date.DayOfYear;
        }

        public static DateTime ToPST(this DateTime dateTime)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
    }
}
