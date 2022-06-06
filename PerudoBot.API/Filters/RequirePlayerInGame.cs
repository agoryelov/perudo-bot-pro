using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PerudoBot.API.Services;

namespace PerudoBot.API.Filters
{
    public class RequirePlayerInGame : RequireGameInProgress
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;

            if (!headers.ContainsKey("PLAYER_ID"))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!int.TryParse(headers["PLAYER_ID"], out int playerId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            base.OnActionExecuting(context);

            var gameService = context.HttpContext.RequestServices.GetService<GameService>();
            var player = gameService.LoadActivePlayer(playerId);

            if (player == null)
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (LoadedGame == null)
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!LoadedGame.Players.Any(x => x.Id == player.Id))
            {
                context.Result = new BadRequestResult();
                return;
            }
        }
    }
}
