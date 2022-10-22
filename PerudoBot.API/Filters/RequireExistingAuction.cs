using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Filters
{
    public class RequireExistingAuction : ActionFilterAttribute
    {
        protected Auction LoadedAuction;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var headers = context.HttpContext.Request.Headers;

            if (!headers.ContainsKey("AUCTION_ID"))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!int.TryParse(headers["AUCTION_ID"], out int auctionId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var auctionService = context.HttpContext.RequestServices.GetService<AuctionService>();

            LoadedAuction = auctionService.LoadActiveAuction(auctionId);

            if (LoadedAuction == null)
            {
                context.Result = new BadRequestResult();
                return;
            }            
        }
    }
}
