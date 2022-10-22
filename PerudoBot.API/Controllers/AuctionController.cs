using Microsoft.AspNetCore.Mvc;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Filters;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Controllers
{
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionService _auctionService;
        private readonly UserService _userService;

        public AuctionController(AuctionService auctionService, UserService userService)
        {
            _auctionService = auctionService;
            _userService = userService;
        }

        [HttpPost]
        [Route("auction/start")]
        public IResult StartAuction(AuctionSetup setup)
        {
            var response = _auctionService.StartAuction(setup);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = _auctionService.GetCurrentAuction() });
        }

        //[HttpGet]
        //[Route("auction")]
        //[RequireExistingAuction]
        //public IResult CurrentRoundState()
        //{
        //    var response = _auctionService.GetCurrentAuction();

        //    if (!response.RequestSuccess)
        //    {
        //        return Results.BadRequest(new { error = response.ErrorMessage });
        //    }

        //    return Results.Ok(new { data = response });
        //}

        [HttpPost]
        [Route("auction/bid")]
        [RequireAuctionInProgress]
        public IResult BidAction(AuctionBidAttempt bid)
        {
            var user = _userService.GetUserFromDiscordId(bid.DiscordId);

            if (user == null) 
            {
                return Results.BadRequest(new { error = "User not found" });
            }

            var response = _auctionService.AddAuctionBid(user, bid.Amount);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = _auctionService.GetCurrentAuction() });
        }

        [HttpPost]
        [Route("auction/pass")]
        [RequireAuctionInProgress]
        public IResult PassAction(AuctionPassAttempt pass)
        {
            var user = _userService.GetUserFromDiscordId(pass.DiscordId);

            if (user == null)
            {
                return Results.BadRequest(new { error = "User not found" });
            }

            var response = _auctionService.AddAuctionPass(user);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = _auctionService.GetCurrentAuction() });
        }

        [HttpPost]
        [Route("auction/end")]
        [RequireAuctionInProgress]
        public IResult EndGame()
        {
            _auctionService.EndAuction();

            return Results.Ok(new { data = _auctionService.AuctionSummary() });
        }
    }
}
