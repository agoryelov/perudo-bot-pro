import discord
from models import GameLog

class GameLogEmbed(discord.Embed):
    def __init__(self, game_log: GameLog):
        super().__init__()
        num_games = len(game_log.recent_games)
        self.title = f"Recent Games ({num_games})"
        self.description = ""

        for game in game_log.recent_games:
            self.description += f"\n`#{game.id} {game.date}` *{game.state}*"

