import discord
import game

from discord import SelectOption
from models import Round, GameSetup
from utils import GameActionError

class GameSetupView(discord.ui.View):
    def __init__(self, game_driver):
        super().__init__(timeout=600)
        self.game_driver : game.GameDriver = game_driver
        self.player_count = 0
        self.first_round : Round = None
        self.timed_out = True

    @discord.ui.button(label='Join', style=discord.ButtonStyle.gray)
    async def add(self, interaction: discord.Interaction, button: discord.ui.Button):   
        try:
            game_setup = await self.game_driver.add_player(interaction.user)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)
            return
        
        self.player_count += 1

        if interaction.channel.last_message_id == interaction.message.id:
            await interaction.response.edit_message(embed=GameSetupEmbed(game_setup))
        else:
            await interaction.message.delete()
            await interaction.channel.send(view=self, embed=GameSetupEmbed(game_setup))
    
    @discord.ui.select(placeholder="Choose game mode", options=[ SelectOption(label='Sudden Death', value='0'), SelectOption(label='Reverse', value='1') ])
    async def mode(self, interaction: discord.Interaction, select: discord.ui.Select):
        game_setup = await self.game_driver.set_default_round_type(select.values[0])

        if interaction.channel.last_message_id == interaction.message.id:
            await interaction.response.edit_message(view=self, embed=GameSetupEmbed(game_setup))
        else:
            await interaction.message.delete()
            await interaction.channel.send(view=self, embed=GameSetupEmbed(game_setup))

    @discord.ui.button(label='Start', style=discord.ButtonStyle.green)
    async def start(self, interaction: discord.Interaction, button: discord.ui.Button):
        if self.player_count < 2:
            await interaction.response.send_message("Need at least two players to start", ephemeral=True)
        else:
            self.timed_out = False
            self.clear_items()
            await interaction.response.edit_message(view=self)
            self.stop()

    @discord.ui.button(label='Voice', style=discord.ButtonStyle.gray)
    async def join_voice(self, interaction: discord.Interaction, button: discord.ui.Button):
        if interaction.user.voice is None:
            await interaction.response.send_message('You are not in a voice channel', ephemeral=True)
            return
        
        voice_channel = interaction.user.voice.channel
        try:
            voice_client = await voice_channel.connect()
        except:
            await interaction.response.send_message(f'Unable to connect to the {voice_channel.name}', ephemeral=True)
            return

        self.game_driver.voice_client = voice_client
        button.disabled = True
        await interaction.response.edit_message(view=self)

    @discord.ui.button(label='BeginnerBot', style=discord.ButtonStyle.gray, row=1)
    async def beginner_bot(self, interaction: discord.Interaction, button: discord.ui.Button):
        try:
            bot_user = await interaction.guild.fetch_member('743151009689501818')
            game_setup = await self.game_driver.add_player(bot_user)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)
            return

        self.player_count += 1
        button.disabled = True

        if interaction.channel.last_message_id == interaction.message.id:
            await interaction.response.edit_message(view=self, embed=GameSetupEmbed(game_setup))
        else:
            await interaction.message.delete()
            await interaction.channel.send(view=self, embed=GameSetupEmbed(game_setup))

class GameSetupEmbed(discord.Embed):
    def __init__(self, game_setup: GameSetup):
        super().__init__()
        self.setup = game_setup
        self.title = f'New Game'
        self.add_field(name=f"Players ({len(self.setup.players)})", value=self.get_setup_players_field(), inline=False)
        self.add_field(name="Game Mode", value=self.setup.round_type, inline=False)
    
    def get_setup_players_field(self):
        setup_players = []
        for player in self.setup.players.values():
            setup_players.append(f'{player.name} `{player.points} pts`')
        if len(setup_players) == 0: return "No players yet"
        return '\n'.join(setup_players)