from typing import List
import discord
from game.client import GameClient
from models import UserInventory, DiceItem
from utils import GameActionError, dice_preview, dice_emote, EMPTY

from itertools import groupby


class EquipItemSelect(discord.ui.Select):
    def __init__(self, items : List[DiceItem]):
        options = [
            discord.SelectOption(label="Default Dice", value='-1', emoji=dice_emote(1))
        ]

        sorted_items = sorted(items, key=lambda x: x.item_name)
        for _, g in groupby(sorted_items, key=lambda x: x.item_name):
            item = list(g)[0]
            options.append(discord.SelectOption(label=item.item_name, value=item.item_id, emoji=dice_emote(1, item.emotes)))

        super().__init__(placeholder="Equip item...", max_values=1, min_values=1, options=options)
    
    async def callback(self, interaction: discord.Interaction):
        try:
            GameClient.equip_dice_item(interaction.user.id, item_id=int(self.values[0]))
            inventory_data = GameClient.get_user_inventory(interaction.user.id)
            await interaction.response.edit_message(embed=UserInventoryEmbed(UserInventory(inventory_data)))
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class UserInventoryView(discord.ui.View):
    def __init__(self, inventory: UserInventory):
        super().__init__()
        self.add_item(EquipItemSelect(inventory.dice_items))

class UserInventoryEmbed(discord.Embed):
    def __init__(self, inventory: UserInventory):
        super().__init__()
        self.title = "My Inventory"
        self.description = self.get_equipped_dice(inventory.equipped_dice)

        sorted_items = sorted(inventory.dice_items, key=lambda x: x.item_name)
        for _, g in groupby(sorted_items, key=lambda x: x.item_name):
            group = list(g)
            self.add_dice_field(group[0], len(group))

    def get_equipped_dice(self, equipped_dice: DiceItem):
        if equipped_dice is None:
            return f'Equipped: **Default Dice** \n{dice_preview()}\n'
        else:
            return f'Equipped: **{equipped_dice.item_name}** \n{dice_preview(equipped_dice.emotes)}\n'
    
    def add_dice_field(self, item: DiceItem, count: int):
        self.add_field(
            name = EMPTY,
            value = f'`{count}x` {item.item_name} \n{dice_preview(item.emotes)}',
            inline = False
        )