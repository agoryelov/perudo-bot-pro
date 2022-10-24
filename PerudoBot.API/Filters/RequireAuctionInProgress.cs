using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PerudoBot.API.Constants;

namespace PerudoBot.API.Filters
{
    public class RequireAuctionInProgress : RequireExistingAuction
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (LoadedAuction?.State != (int)AuctionState.InProgress)
            {
                context.Result = new BadRequestResult();
                return;
            }
        }
    }
}
