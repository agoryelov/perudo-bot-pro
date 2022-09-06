using Microsoft.AspNetCore.Mvc;
using PerudoBot.API.DTOs;
using PerudoBot.API.Services;

namespace PerudoBot.API.Controllers
{
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AchievementService _achievementService;

        public GeneralController(UserService userService, AchievementService achievementService)
        {
            _userService = userService;
            _achievementService = achievementService;
        }

        [HttpGet]
        [Route("general/ladder")]
        public IResult GetLadderInfo()
        {
            var response = _userService.GetLadderInfo();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpGet]
        [Route("general/achievements")]
        public IResult GetAchievements()
        {
            var achievements = _achievementService.GetAchievementDetails();

            if (achievements.Count == 0)
            {
                return Results.BadRequest(new { error = "No achievements" });
            }

            return Results.Ok(new { data = achievements });
        }

        [HttpGet]
        [Route("general/achievements/{discordId}")]
        public IResult GetUserAchievements(ulong discordId)
        {
            var user = _userService.GetUserFromDiscordId(discordId);

            if (user == null)
            {
                return Results.BadRequest(new { error = "User is not recognized" });
            }

            var achievements = _achievementService.GetAchievementsForUser(user);

            if (achievements.Count == 0)
            {
                return Results.BadRequest(new { error = "No achievements for user" });
            }

            return Results.Ok(new { data = achievements });
        }


        [HttpGet]
        [Route("general/profile/{discordId}")]
        public IResult GetUserProfile(ulong discordId)
        {
            var response = _userService.GetUserProfile(discordId);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("general/rattles")]
        public IResult UpdateRattle(RattleUpdate rattleUpdate)
        {
            var response = _userService.UpdateRattle(rattleUpdate);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }
    }
}
