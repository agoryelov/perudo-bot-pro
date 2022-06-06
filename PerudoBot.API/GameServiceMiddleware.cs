using PerudoBot.API.Services;

namespace PerudoBot.API
{
    public class GameServiceMiddleware
    {
        private readonly RequestDelegate _next;

        public GameServiceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, GameService gameService)
        {
            if (context.Request.Headers.ContainsKey("GAME_ID"))
            {
                var requestGameId = int.Parse(context.Request.Headers["GAME_ID"]);
                gameService.LoadActiveGame(requestGameId);
            }

            if (context.Request.Headers.ContainsKey("PLAYER_ID"))
            {
                var requestPlayerId = int.Parse(context.Request.Headers["PLAYER_ID"]);
                gameService.LoadActivePlayer(requestPlayerId);
            }

            await _next(context);
        }
    }
}
