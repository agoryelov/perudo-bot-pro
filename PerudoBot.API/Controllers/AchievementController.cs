using Microsoft.AspNetCore.Mvc;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Filters;
using PerudoBot.API.Services;

namespace PerudoBot.API.Controllers
{
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly AchievementService _achievementService;

        public AchievementController(AchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        //[HttpGet]
        //[Route("achievements/new")]
        //public IResult GetNewAchievements()
        //{
        //    var response = _achievementService.GetNewAchievements();

        //    if (!response.RequestSuccess)
        //    {
        //        return Results.BadRequest(new { error = response.ErrorMessage });
        //    }

        //    return Results.Ok(new { data = response });
        //}
    }
}
