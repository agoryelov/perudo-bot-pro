using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Filters
{
    public class RequireExistingGame : ActionFilterAttribute
    {
        protected Game LoadedGame;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var headers = context.HttpContext.Request.Headers;

            if (!headers.ContainsKey("GAME_ID"))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!int.TryParse(headers["GAME_ID"], out int gameId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var gameService = context.HttpContext.RequestServices.GetService<GameService>();

            LoadedGame = gameService.LoadActiveGame(gameId);

            if (LoadedGame == null)
            {
                context.Result = new BadRequestResult();
                return;
            }            
        }
    }
}
