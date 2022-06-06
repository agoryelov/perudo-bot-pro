using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PerudoBot.API.Constants;
using PerudoBot.API.Services;

namespace PerudoBot.API.Filters
{
    public class RequireCompletedGame : RequireExistingGame
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (LoadedGame?.State != (int)GameState.Ended)
            {
                context.Result = new BadRequestResult();
                return;
            }
        }
    }
}
