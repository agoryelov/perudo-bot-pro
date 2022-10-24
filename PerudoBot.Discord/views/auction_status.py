import discord
from models import Auction

from typing import TYPE_CHECKING

from utils import format_points, dice_preview, GameActionError

if TYPE_CHECKING:
    from services import PerudoContext

class RaiseButton(discord.ui.Button['AuctionView']):
    def __init__(self, amount):
        super().__init__(label=f'{amount}', style=discord.ButtonStyle.gray)
        self.amount = amount
    
    async def callback(self, interaction: discord.Interaction):
        auction_service = self.view.ctx.auction

        try:
            await interaction.response.defer()
            highest_bid = auction_service.auction.current_amount
            auction = await auction_service.auction_bid(interaction.user.id, highest_bid + self.amount)
            await self.view.ctx.update_auction_message(auction)

            if auction.is_completed:
                await interaction.message.edit(view=None)
                await auction_service.end_auction()
        except GameActionError as e:
            if not interaction.user.bot: await interaction.user.send(f"`Bid Failed`: {e.message}")

class EverythingButton(discord.ui.Button['AuctionView']):
    def __init__(self):
        super().__init__(emoji='♾️', style=discord.ButtonStyle.gray)
    
    async def callback(self, interaction: discord.Interaction):
        auction_service = self.view.ctx.auction
        try:
            await interaction.response.defer()
            player = auction_service.get_player(interaction.user.id)
            auction = await auction_service.auction_bid(interaction.user.id, player.points)
            await self.view.ctx.update_auction_message(auction)

            if auction.is_completed:
                await interaction.message.edit(view=None)
                await auction_service.end_auction()
        except GameActionError as e:
            if not interaction.user.bot: await interaction.user.send(f"`Bid Failed`: {e.message}")

class PassButton(discord.ui.Button['AuctionView']):
    def __init__(self):
        super().__init__(label='Pass', style=discord.ButtonStyle.red)
    
    async def callback(self, interaction: discord.Interaction):
        auction_service = self.view.ctx.auction
        try:
            await interaction.response.defer()
            auction = await auction_service.auction_pass(interaction.user.id)
            await self.view.ctx.update_auction_message(auction)

            if auction.is_completed:
                await interaction.message.edit(view=None)
                await auction_service.end_auction()
        except GameActionError as e:
            if not interaction.user.bot: await interaction.user.send(f"`Pass Failed`: {e.message}")

class AuctionView(discord.ui.View):
    def __init__(self, ctx: 'PerudoContext'):
        super().__init__(timeout=1200)
        self.ctx = ctx
        self.auction = ctx.auction.auction

        self.add_item(RaiseButton(50))
        self.add_item(RaiseButton(250))
        self.add_item(EverythingButton())
        self.add_item(PassButton())

class AuctionEmbed(discord.Embed):
    def __init__(self, a: Auction):
        super().__init__(title=f'Auction')
        self.auction = a
        self.description = self.get_description()
        self.set_footer(text=self.get_footer())

        self.add_field(name='Players', value=self.get_players_field())

        if self.auction.any_passed:
            self.add_field(name="Passed", value=self.get_passed_field())

        if self.auction.any_actions:
            self.add_field(name="Auction Log", value=self.get_auctionlog(), inline=False)

    def get_description(self):
        item = self.auction.item
        return f'{item.name} {format_points(item.price)}\n{dice_preview(item.content)}'

    def get_footer(self):
        if self.auction.is_completed or not self.auction.any_bids:
            return ''
        
        current_bid = self.auction.highest_bid
        bidder = self.auction.players[current_bid.player_id]
        return f'Current Bid: {bidder.name} @ {current_bid.bid_amount}'

    def get_players_field(self):
        active_players = []
        for player in self.auction.players.values():
            if player.is_active:
                active_players.append(f'{player.avatar} {player.name} {format_points(player.points)}')
        return '\n'.join(active_players)
    
    def get_passed_field(self):
        eliminated = []
        for player in self.auction.players.values():
            if not player.is_active:
                eliminated.append(f':flag_white: {player.name} {format_points(player.points)}')
        if len(eliminated) == 0: return 'None'
        return '\n'.join(eliminated)

    def get_auctionlog(self):
        logs = []
        for action in self.auction.actions:
            time = action.date_created.strftime('%H:%M')
            player = self.auction.players[action.player_id]
            if action.is_pass:
                logs.append(f'`{time}`: {player.name} passed')
            else:
                logs.append(f'`{time}`: {player.name} bids :dollar: `{action.bid_amount}`')
        if len(logs) == 0: return 'None'
        return '\n'.join(logs[-10:])