import asyncio
from typing import TYPE_CHECKING

from discord.ext import commands
from utils import MessageType
from models import Auction, Round, GameSetup
from views import AuctionEmbed, AuctionView, RoundEmbed, RoundView, GameSetupView, GameSetupEmbed, BetsView, BetsEmbed

if TYPE_CHECKING:
    from bot import PerudoBot

class PerudoContext(commands.Context):
    bot: 'PerudoBot'

    def __init__(self, **kwargs):
        super().__init__(**kwargs)

    @property
    def is_slash(self) -> bool:
        return self.interaction is not None
    
    @property
    def game(self):
        return self.bot.channel_game(self)

    @property
    def auction(self):
        return self.bot.channel_auction(self)
    
    async def clear_message(self, type: MessageType):
        message = self.bot.get_message(self, type=type)
        if message is not None: await message.edit(view=None)
    
    async def send_auction_message(self, auction: Auction):
        auction_message = await self.send_delayed(view=AuctionView(self), embed=AuctionEmbed(auction))
        self.bot.set_message(self, type=MessageType.Auction, message=auction_message)
    
    async def send_round_message(self, round: Round):
        round_message = await self.send_delayed(view=RoundView(self), embed=RoundEmbed(round))
        self.bot.set_message(self, type=MessageType.Round, message=round_message)

    async def send_bets_message(self, round: Round):
        bets_message = await self.send_delayed(view=BetsView(self), embed=BetsEmbed(round), delay=0)
        self.bot.set_message(self, type=MessageType.Bets, message=bets_message)
    
    async def send_setup_message(self, setup: GameSetup):
        setup_message = await self.send_delayed(view=GameSetupView(self), embed=GameSetupEmbed(setup))
        self.bot.set_message(self, type=MessageType.Setup, message=setup_message)
    
    async def update_auction_message(self, auction: Auction, edit_function = None):
        await self._update_message(type=MessageType.Auction, edit_function=edit_function, view=AuctionView(self), embed=AuctionEmbed(auction))
    
    async def update_round_message(self, round: Round, edit_function = None):
        await self._update_message(type=MessageType.Round, edit_function=edit_function, view=RoundView(self), embed=RoundEmbed(round))

    async def update_bets_message(self, round: Round, edit_function = None):
        await self._update_message(type=MessageType.Bets, edit_function=edit_function, view=BetsView(self), embed=BetsEmbed(round))

    async def update_setup_message(self, setup: GameSetup, edit_function = None):
        await self._update_message(type=MessageType.Setup, edit_function=edit_function, view=GameSetupView(self), embed=GameSetupEmbed(setup))
    
    async def _update_message(self, type: MessageType, edit_function, **kwargs):
        message = self.bot.get_message(self, type=type)
        edit_function = edit_function or message.edit
        recent_history = [message.id async for message in self.channel.history(limit=2)]

        if message.id in recent_history:
            await edit_function(**kwargs)
        else:
            await message.delete()
            message = await self.channel.send(**kwargs)
            self.bot.set_message(self, type=type, message=message)
    
    async def send_delayed(self, delay = 0.5, **kwargs):
        await asyncio.sleep(delay)
        return await self.channel.send(**kwargs)