import asyncio
import typing

from discord import Message

from views import AuctionSummaryEmbed
from models import Auction, AuctionSummary

from .client import Client

if typing.TYPE_CHECKING:
    from .context import PerudoContext

class AuctionService():
    def __init__(self, ctx: 'PerudoContext'):
        self.ctx = ctx
        self.auction: Auction
        self.auction_message : Message

    async def start_auction(self, auction_setup) -> Auction:
        auction_data = Client.start_auction(auction_setup)
        auction = Auction(auction_data)
        await self._update_from_auction(auction)
        return auction

    async def auction_bid(self, discord_id, amount):
        auction_data = Client.auction_bid(self.auction.id, discord_id, amount)
        auction = Auction(auction_data)
        await self._update_from_auction(auction)
        return auction

    async def auction_pass(self, discord_id):
        auction_data = Client.auction_pass(self.auction.id, discord_id)
        auction = Auction(auction_data)
        await self._update_from_auction(auction)
        return auction

    async def end_auction(self):
        summary_data = Client.end_auction(self.auction.id)
        auction_summary = AuctionSummary(summary_data)
        
        await self._send_auction_summary(auction_summary)
        await asyncio.sleep(2)
        await self._resend_game_setup()

    async def _send_auction_summary(self, auction_summary: AuctionSummary):
        await self.ctx.send_delayed(embed=AuctionSummaryEmbed(auction_summary))
    
    async def _resend_game_setup(self):
        game_setup = await self.ctx.game.fetch_setup()
        await self.ctx.send_setup_message(game_setup)

    async def _update_from_auction(self, auction: Auction):
        self.auction = auction

    def get_max_bid(self, discord_id):
        player_points, max_points = 0, 0
        for player in self.auction.players.values():
            if not player.is_active: continue
            if player.discord_id == discord_id: 
                player_points = player.points
            else: 
                max_points = max(max_points, player.points)
        return min(player_points, max_points + 10)