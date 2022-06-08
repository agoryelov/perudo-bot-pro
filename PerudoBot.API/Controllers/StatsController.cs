using Microsoft.AspNetCore.Mvc;
using PerudoBot.API.Services;

namespace PerudoBot.API.Controllers
{
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly UserService _userService;

        public StatsController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("stats/ladder")]
        public IResult GetLadderInfo()
        {
            var response = _userService.GetLadderInfo();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }
    }
}
