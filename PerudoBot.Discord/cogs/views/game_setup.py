import discord
from cogs.models.round import Round
from cogs.models.setup import GameSetup
from cogs.utils.client import GameClient

class GameSetupView(discord.ui.View):
    def __init__(self, game_client):
        super().__init__()
        self.game_client : GameClient = game_client
        self.player_count = 0
        self.first_round : Round = None

    @discord.ui.button(label='Add', style=discord.ButtonStyle.gray)
    async def add(self, interaction: discord.Interaction, button: discord.ui.Button):
        player_name = interaction.user.nick if interaction.user.nick is not None else interaction.user.name

        response = self.game_client.add_player(interaction.user.id, player_name)
        
        if not response.is_success:
            await interaction.response.send_message(response.error_message, ephemeral=True)
            return
        
        game_setup = GameSetup(response.data)
        self.player_count += 1

        if interaction.channel.last_message_id == interaction.message.id:
            await interaction.response.edit_message(embed=GameSetupEmbed(game_setup))
        else:
            await interaction.message.delete()
            await interaction.channel.send(view=self, embed=GameSetupEmbed(game_setup))
    
    @discord.ui.select(placeholder="Choose game mode", options=[
        discord.SelectOption(label='Sudden Death', value='0'),
        discord.SelectOption(label='Reverse', value='1')
    ])
    async def mode(self, interaction: discord.Interaction, select: discord.ui.Select):
        response = self.game_client.set_default_round_type(select.values[0])

        if not response.is_success:
            await interaction.response.send_message(response.error_message, ephemeral=True)
            return

        game_setup = GameSetup(response.data)

        if interaction.channel.last_message_id == interaction.message.id:
            await interaction.response.edit_message(embed=GameSetupEmbed(game_setup))
        else:
            await interaction.message.delete()
            await interaction.channel.send(view=self, embed=GameSetupEmbed(game_setup))

    @discord.ui.button(label='Start', style=discord.ButtonStyle.green)
    async def start(self, interaction: discord.Interaction, button: discord.ui.Button):
        if self.player_count < 2:
            await interaction.response.send_message("Need at least two players to start", ephemeral=True)
        else:
            self.clear_items()
            await interaction.response.edit_message(view=self)
            self.stop()
        

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