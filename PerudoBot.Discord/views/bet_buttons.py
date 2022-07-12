from typing import TYPE_CHECKING
import discord
from models import Round
from utils import BetType, GameActionError, bet_emoji

if TYPE_CHECKING:
    from game import GameDriver

class BetButton(discord.ui.Button['BetButtonsView']):
    def __init__(self, bet_type: BetType, bet_amount: int, row: int):
        super().__init__(label=f'{bet_amount}', row=row, style=discord.ButtonStyle.gray)
        self.emoji = bet_emoji(bet_type)
        self.bet_type = bet_type
        self.bet_amount = bet_amount

    async def callback(self, interaction: discord.Interaction):
        game_driver = self.view.game_driver
        try:
            r = await game_driver.bet_action(interaction.user.id, self.bet_amount, self.bet_type)
            await game_driver.update_round_message(r)
            await interaction.response.edit_message()
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class BetButtonsView(discord.ui.View):
    def __init__(self, r: Round, game_driver: 'GameDriver', bet_type: BetType):
        super().__init__(timeout=1200)
        self.round = r
        self.game_driver = game_driver

        self.add_item(BetButton(bet_type, 100, row=0))
        self.add_item(BetButton(bet_type, 1000, row=0))
        self.add_item(BetButton(bet_type, 5000, row=0))