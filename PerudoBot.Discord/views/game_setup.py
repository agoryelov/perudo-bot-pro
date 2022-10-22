import typing
import discord
from discord import SelectOption
from models import GameSetup
from utils import GameActionError, GameSetupAction, dice_preview, format_points

if typing.TYPE_CHECKING:
    from services import PerudoContext

class JoinButton(discord.ui.Button['GameSetupView']):
    def __init__(self):
        super().__init__(label='Join', style=discord.ButtonStyle.gray)
    
    async def callback(self, interaction: discord.Interaction):
        try:
            game_setup = await self.view.game.add_player(interaction.user)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)
            return

        await self.view.ctx.update_setup_message(game_setup, interaction.response.edit_message)

class GameButton(discord.ui.Button['GameSetupView']):
    def __init__(self):
        super().__init__(label='Start Game', style=discord.ButtonStyle.green)
    
    async def callback(self, interaction: discord.Interaction):
        self.view.game.set_voice_channel(interaction.user)
        if not self.view.setup.can_start_game():
            await interaction.response.send_message("Need at least two players to start a game", ephemeral=True)
            return
        
        try:
            self.setup_action = GameSetupAction.StartGame
            round = await self.view.game.start_game()
            if self.view.game.has_bots: await self.view.game.send_bot_updates(round)

            await self.view.ctx.bot.clear_active_view()
            await self.view.ctx.send_round_message(round)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class AuctionButton(discord.ui.Button['GameSetupView']):
    def __init__(self):
        super().__init__(label='Auction', style=discord.ButtonStyle.green)
    
    async def callback(self, interaction: discord.Interaction):
        if not self.view.setup.can_start_auction(interaction.user.id):
            await interaction.response.send_message("Need at least two eligible players to start an auction", ephemeral=True)
            return

        try:
            self.setup_action = GameSetupAction.StartAuction
            auction_setup = self.view.setup.auction_setup(interaction.user.id)
            auction = await self.view.auction.start_auction(auction_setup)

            await self.view.ctx.bot.clear_active_view()
            await self.view.ctx.send_auction_message(auction)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class GameSetupView(discord.ui.View):
    def __init__(self, ctx: 'PerudoContext'):
        super().__init__(timeout=600)
        self.ctx = ctx
        self.game = ctx.game
        self.auction = ctx.auction
        self.setup = ctx.game.game_setup

        self.add_item(JoinButton())
        self.add_item(GameButton())

        if self.setup.show_auction:
            self.add_item(AuctionButton())
    
    @discord.ui.select(placeholder="Choose game mode", options=[ SelectOption(label='Sudden Death', value='0'), SelectOption(label='Reverse', value='1') ], row=1)
    async def mode(self, interaction: discord.Interaction, select: discord.ui.Select):
        try:
            game_setup = await self.game.set_default_round_type(select.values[0])
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)
            return
        
        await self.ctx.update_setup_message(game_setup, interaction.response.edit_message)

    async def on_timeout(self):
        await self.ctx.game.terminate()
        await self.ctx.bot.clear_active_view(text='Lobby terminated due to inactivity')

class GameSetupEmbed(discord.Embed):
    def __init__(self, game_setup: GameSetup):
        super().__init__()
        self.setup = game_setup
        self.title = f'Lobby'
        self.add_field(name=f"Players ({len(self.setup.players)})", value=self.get_setup_players_field(), inline=False)
        self.add_field(name="Game Mode", value=self.setup.round_type, inline=False)

        if game_setup.show_auction: 
            self.add_field(name='Daily Auction', value=self.get_daily_auction(), inline=False)
    
    def get_setup_players_field(self):
        setup_players = []
        for player in self.setup.players.values():
            setup_players.append(f'{player.avatar} {player.name} {format_points(player.points)}')
        if len(setup_players) == 0: return "No players yet"
        return '\n'.join(setup_players)

    def get_daily_auction(self):
        item = self.setup.auction_item
        return f'{item.name} {format_points(item.price)}\n{dice_preview(item.content)}'