using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Services
{
    public class AuctionService
    {
        private Auction _activeAuction { get; set; }

        private readonly PerudoBotDbContext _db;
        private readonly ItemService _itemService;
        private readonly UserService _userService;

        public AuctionService(PerudoBotDbContext context, ItemService itemService, UserService userService)
        {
            _db = context;
            _itemService = itemService;
            _userService = userService;
        }

        public bool IsAvailable()
        {
            var itemPool = _db.Items
               .Where(x => x.DropEnabled)
               .Select(x => x.Id)
               .ToList();

            return itemPool.Count > 5;
        }

        public Auction LoadActiveAuction(int auctionId)
        {
            _activeAuction = _db.Auctions
                .Where(x => x.Id == auctionId)
                .Include(x => x.AuctionPlayers)
                    .ThenInclude(x => x.User)
                        .ThenInclude(x => x.UserItems)
                .Include(x => x.AuctionActions)
                .Include(x => x.AuctionItem)
                .SingleOrDefault();

            return _activeAuction;
        }

        private Auction CreateDailyAuction(int auctionDay)
        {
            var recentItems = _db.Auctions
                .Where(x => x.State == (int)AuctionState.Ended)
                .OrderByDescending(x => x.Day).Take(5)
                .Select(x => x.AuctionItemId)
                .ToList();

            var itemPool = _db.Items
                .Where(x => x.DropEnabled)
                .Select(x => x.Id)
                .ToList();

            itemPool = itemPool.Except(recentItems).ToList();

            var rng = new Random();
            var randomIndex = rng.Next(itemPool.Count);

            var randomItem = _db.Items.SingleOrDefault(x => x.Id == itemPool[randomIndex]);

            return new Auction
            {
                Day = auctionDay,
                State = (int)AuctionState.Setup,
                AuctionPlayers = new List<AuctionPlayer>(),
                AuctionActions = new List<AuctionAction>(),
                AuctionItem = randomItem,
            };
        }

        public Auction GetDailyAuction()
        {
            var auctionDay = DateTime.Now.AuctionDay();
            var dailyAuction = _db.Auctions.Include(x => x.AuctionItem).FirstOrDefault(x => x.Day == auctionDay);

            if (dailyAuction == null)
            {
                dailyAuction = CreateDailyAuction(auctionDay);

                _db.Auctions.Add(dailyAuction);
                _db.SaveChanges();
            }

            return dailyAuction;
        }

        private AuctionPlayer CreateAuctionPlayer(ulong discordId)
        {
            var user = _db.Users.SingleOrDefault(x => x.DiscordId == discordId);
            var player = _userService.GetMostRecentPlayer(user);

            return new AuctionPlayer
            {
                IsActive = (user.Points >= _activeAuction.AuctionItem.AuctionPrice()) && !user.IsBot,
                AuctionId = _activeAuction.Id,
                UserId = user.Id,
                GamePlayerId = player?.Id
            };
        }

        public Response StartAuction(AuctionSetup setup)
        {
            LoadActiveAuction(setup.AuctionId);

            foreach (var discordId in setup.DiscordIds)
            {
                var auctionPlayer = CreateAuctionPlayer(discordId);
                _activeAuction.AuctionPlayers.Add(auctionPlayer);
            }

            _db.SaveChanges();

            if (_activeAuction.ActivePlayers.Count < 2)
            {
                _activeAuction.State = (int) AuctionState.Terminated;
                _db.SaveChanges();

                return Responses.Error("Not enough eligible users to start an auction");
            }

            _activeAuction.State = (int) AuctionState.InProgress;

            var startingUser = _userService.GetUserFromDiscordId(setup.StartingDiscordId);
            var response = AddAuctionBid(startingUser, _activeAuction.AuctionItem.AuctionPrice());

            return response;
        }

        private void UpdatePassedPlayers()
        {
            var highestBid = _activeAuction.HighestBid;
            var highestBidAmount = _activeAuction.HighestBidAmount();

            foreach (var player in _activeAuction.AuctionPlayers)
            {
                var isHighestBidder = highestBid != null && highestBid.AuctionPlayerId == player.Id;
                var isEligibleToBid = player.User.Points > highestBidAmount;

                if (!isEligibleToBid && !isHighestBidder)
                {
                    player.IsActive = false;
                }
            }

            _db.SaveChanges();
        }

        private AuctionPlayer GetAuctionPlayer(User user)
        {
            var player = _activeAuction.AuctionPlayers.SingleOrDefault(x => x.User.Id == user.Id);
            if (player != null) return player;

            var auctionPlayer = new AuctionPlayer
            {
                UserId = user.Id,
                AuctionId = _activeAuction.Id,
                IsActive = user.Points > _activeAuction.HighestBidAmount()
            };

            _activeAuction.AuctionPlayers.Add(auctionPlayer);
            _db.SaveChanges();

            return auctionPlayer;
        }

        public Response AddAuctionBid(User user, int amount)
        {
            var auctionPlayer = GetAuctionPlayer(user);

            if (!auctionPlayer.IsActive)
            {
                return Responses.Error("Player not currently active");
            }

            if (amount > auctionPlayer.User.Points)
            {
                amount = auctionPlayer.User.Points;
            }

            var itemPrice = _activeAuction.AuctionItem.AuctionPrice();
            var highestBid = _activeAuction.HighestBid;

            if (!highestBid.IsValidFollowUp(amount, itemPrice))
            {
                return Responses.Error("Invalid bid attempt");
            }

            var auctionBid = new AuctionBid
            {
                AuctionId = _activeAuction.Id,
                AuctionPlayerId = auctionPlayer.Id,
                ParentActionId = _activeAuction.LatestAction?.Id,
                BidAmount = amount
            };

            _activeAuction.AuctionActions.Add(auctionBid);
            _db.SaveChanges();

            return Responses.OK();
        }

        public Response AddAuctionPass(User user)
        {
            var auctionPlayer = GetAuctionPlayer(user);

            if (!auctionPlayer.IsActive)
            {
                return Responses.Error("Player not currently active");
            }

            if (_activeAuction.HighestBid.AuctionPlayer.Id == auctionPlayer.Id)
            {
                return Responses.Error("Can't pass when leading the auction");
            }

            auctionPlayer.IsActive = false;

            var auctionPass = new AuctionPass
            {
                AuctionId = _activeAuction.Id,
                AuctionPlayerId = auctionPlayer.Id,
                ParentActionId = _activeAuction.LatestAction?.Id
            };
            
            _activeAuction.AuctionActions.Add(auctionPass);
            _db.SaveChanges();

            return Responses.OK();
        }

        public AuctionDto GetCurrentAuction()
        {
            UpdatePassedPlayers();
            return _activeAuction.ToAuctionDto();
        }

        public AuctionSummaryDto AuctionSummary()
        {
            return _activeAuction.ToAuctionSummaryDto();
        }

        public void EndAuction()
        {
            _activeAuction.FinalPrice = _activeAuction.HighestBidAmount();
            _activeAuction.State = (int)AuctionState.Ended;

            if (_activeAuction.HighestBid != null)
            {
                _activeAuction.WinningPlayerId = _activeAuction.HighestBid.AuctionPlayerId;

                var winner = _activeAuction.HighestBid.AuctionPlayer.User;
                var finalPrice = _activeAuction.FinalPrice;
                _userService.RemoveAuctionPoints(winner, finalPrice, _activeAuction);
                _itemService.AddItemToUser(winner, _activeAuction.AuctionItemId);
            }

            _db.SaveChanges();
        }
    }
}
