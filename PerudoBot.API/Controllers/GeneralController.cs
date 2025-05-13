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
        private readonly ItemService _itemService;

        public GeneralController(UserService userService, AchievementService achievementService, ItemService itemService)
        {
            _userService = userService;
            _achievementService = achievementService;
            _itemService = itemService;
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

        [HttpGet]
        [Route("general/gamelog")]
        public IResult GetGameLog()
        {
            var test = _userService.RecentGames();
            return Results.Ok(new { data = test, foo = "bar" });
        }

        [HttpGet]
        [Route("general/rattles")]
        public IResult ListRattles()
        {
            return Results.Ok(new { data = _userService.ListRattles() });
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

        [HttpGet]
        [Route("general/inventory/{discordId}")]
        public IResult GetUserInventory(ulong discordId)
        {
            var response = _itemService.GetUserInventory(discordId);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("general/inventory/equip")]
        public IResult EquipDice(ItemUpdate equipItem)
        {
            var user = _userService.GetUserFromDiscordId(equipItem.DiscordId);
            var response = _itemService.EquipDice(user, equipItem.ItemId);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("general/inventory/add")]
        public IResult AddUserItem(ItemUpdate addItem)
        {
            var user = _userService.GetUserFromDiscordId(addItem.DiscordId);
            var response = _itemService.AddItemToUser(user, addItem.ItemId);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("general/inventory/remove")]
        public IResult RemoveUserItem(ItemUpdate removeItem)
        {
            var user = _userService.GetUserFromDiscordId(removeItem.DiscordId);
            var response = _itemService.RemoveItemFromUser(user, removeItem.ItemId);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }
    }
}
