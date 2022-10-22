using Microsoft.EntityFrameworkCore;
using PerudoBot.API.DTOs;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Services
{
    public class ItemService
    {
        private readonly PerudoBotDbContext _db;
        private readonly UserService _userService;

        public ItemService(PerudoBotDbContext context, UserService userService)
        {
            _db = context;
            _userService = userService;
        }

        public UserInventoryDto GetUserInventory(ulong discordId)
        {
            var user = _userService.GetUserFromDiscordId(discordId);

            if (user == null)
            {
                return new UserInventoryDto { RequestSuccess = false, ErrorMessage = "User is not recognized" };
            }

            return user.ToUserInventoryDto();
        }

        public Response EquipDice(User user, int itemId)
        {
            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            // Reset dice to default
            if (itemId == -1)
            {
                user.EquippedDice = null;
                _db.SaveChanges();

                return Responses.OK();
            }

            var item = _db.Items.OfType<DiceItem>().SingleOrDefault(x => x.Id == itemId);

            if (item == null)
            {
                return Responses.Error("Item is not recognized");
            }

            if (!user.UserItems.Any(x => x.Item.Id == item.Id))
            {
                return Responses.Error("User does not own this item");
            }

            user.EquippedDice = item;
            _db.SaveChanges();

            return Responses.OK();
        }

        public Response AddItemToUser(User user, int itemId)
        {
            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            var item = _db.Items.OfType<DiceItem>().SingleOrDefault(x => x.Id == itemId);

            if (item == null)
            {
                return Responses.Error("Item is not recognized");
            }

            user.UserItems.Add(new UserItem
            {
                User = user,
                Item = item
            });

            _db.SaveChanges();

            return Responses.OK();
        }

        public Response RemoveItemFromUser(User user, int itemId)
        {
            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            var item = user.UserItems.FirstOrDefault(x => x.Item.Id == itemId);

            if (item == null)
            {
                return Responses.Error("User doesn't have this item");
            }

            user.UserItems.Remove(item);
            _db.SaveChanges();

            return Responses.OK();
        }
    }
}
