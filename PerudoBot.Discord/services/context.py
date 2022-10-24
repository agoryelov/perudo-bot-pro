import asyncio
from typing import TYPE_CHECKING
# from typing import TYPE_CHECKING

from discord.ext import commands
from models import Auction, Round, GameSetup
from views import AuctionEmbed, AuctionView, RoundEmbed, RoundView, GameSetupView, GameSetupEmbed

if TYPE_CHECKING:
    from bot import PerudoBot

class PerudoContext(commands.Context):
    bot: 'PerudoBot'

    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.game = self.bot.channel_game(self)
        self.auction =  self.bot.channel_auction(self)
        self.active_message = self.bot.get_active_message(self)
    
    async def clear_active_view(self):
        await self.active_message.edit(view=None)
    
    async def send_auction_message(self, auction: Auction):
        self.active_message = await self.send_delayed(view=AuctionView(self), embed=AuctionEmbed(auction))
        self.bot.set_active_message(self, self.active_message)
    
    async def send_round_message(self, round: Round):
        self.active_message = await self.send_delayed(view=RoundView(self), embed=RoundEmbed(round))
        self.bot.set_active_message(self, self.active_message)
    
    async def send_setup_message(self, setup: GameSetup):
        self.active_message = await self.send_delayed(view=GameSetupView(self), embed=GameSetupEmbed(setup))
        self.bot.set_active_message(self, self.active_message)
    
    async def update_auction_message(self, auction: Auction, edit_function = None):
        await self._update_message(edit_function=edit_function, view=AuctionView(self), embed=AuctionEmbed(auction))
    
    async def update_round_message(self, round: Round, edit_function = None):
        await self._update_message(edit_function=edit_function, view=RoundView(self), embed=RoundEmbed(round))

    async def update_setup_message(self, setup: GameSetup, edit_function = None):
        await self._update_message(edit_function=edit_function, view=GameSetupView(self), embed=GameSetupEmbed(setup))
    
    async def _update_message(self, edit_function, **kwargs):
        edit_function = edit_function or self.active_message.edit
        recent_history = [message async for message in self.channel.history(limit=2)]
        if self.active_message in recent_history:
            await edit_function(**kwargs)
        else:
            await self.active_message.delete()
            self.active_message = await self.send_delayed(**kwargs, delay=0)
            self.bot.set_active_message(self, self.active_message)
    
    async def send_delayed(self, delay = 0.5, **kwargs):
        await asyncio.sleep(delay)
        return await self.channel.send(**kwargs)