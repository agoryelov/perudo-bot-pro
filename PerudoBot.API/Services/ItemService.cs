using Microsoft.EntityFrameworkCore;
using PerudoBot.API.DTOs;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Services
{
    public class ItemService
    {
        private readonly PerudoBotDbContext _db;

        public ItemService(PerudoBotDbContext context)
        {
            _db = context;
        }

        private User GetUserWithItems(ulong discordId)
        {
            return _db.Users
                .Where(x => x.DiscordId == discordId)
                .Include(x => x.UserItems)
                    .ThenInclude(x => x.Item)
                .SingleOrDefault();
        }

        public UserInventoryDto GetUserInventory(ulong discordId)
        {
            var user = GetUserWithItems(discordId);

            if (user == null)
            {
                return new UserInventoryDto { RequestSuccess = false, ErrorMessage = "User is not recognized" };
            }

            return user.ToUserInventoryDto();
        }

        public Response EquipDice(ItemUpdate equipItem)
        {
            var user = GetUserWithItems(equipItem.DiscordId);

            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            // Reset dice to default
            if (equipItem.ItemId == -1)
            {
                user.EquippedDice = null;
                _db.SaveChanges();

                return Responses.OK();
            }

            var item = _db.Items.OfType<DiceItem>().SingleOrDefault(x => x.Id == equipItem.ItemId);

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

        public Response AddItemToUser(ItemUpdate addItem)
        {
            var user = GetUserWithItems(addItem.DiscordId);

            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            var item = _db.Items.OfType<DiceItem>().SingleOrDefault(x => x.Id == addItem.ItemId);

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

        public Response RemoveItemFromUser(ItemUpdate removeItem)
        {
            var user = GetUserWithItems(removeItem.DiscordId);

            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            var item = user.UserItems.FirstOrDefault(x => x.Item.Id == removeItem.ItemId);

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
