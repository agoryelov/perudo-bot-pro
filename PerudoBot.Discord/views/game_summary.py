import discord
from models.game import GameSummary

class GameSummaryEmbed(discord.Embed):
    def __init__(self, game_summary: GameSummary):
        super().__init__(title=f'Game {game_summary.game_id} Summary')
        self.game_summary : GameSummary = game_summary
        self.add_field(name="Players", value=self.get_players_field(), inline=False)

        if len(game_summary.betting_changes) > 0:
            self.add_field(name="Betting", value=self.get_betting_field(), inline=False)

        if len(game_summary.notes) > 0:
            self.add_field(name="Notes", value=self.get_notes_field(), inline=False)
        
        if len(game_summary.achievements) > 0:
            self.add_field(name="Achievements", value=self.get_achievements_field(), inline=False)

    def get_players_field(self):
        elo_changes = []
        for id, player in enumerate(self.game_summary.elo_changes):
            elo_changes.append(f'`{id + 1}` {player.name} `{player.starting_elo}` => `{player.final_elo}` ({player.final_elo - player.starting_elo})')
        return '\n'.join(elo_changes)
    
    def get_betting_field(self):
        points_changes = []
        for id, player in enumerate(self.game_summary.betting_changes):
            points_changes.append(f'`{id + 1}` {player.name} `{player.starting_points}` => `{player.final_points}` ({player.final_points - player.starting_points})')
        return '\n'.join(points_changes)
    
    def get_notes_field(self):
        game_notes = []
        for note in self.game_summary.notes:
            game_notes.append(f'**{note.name}** (round {note.round_number}): {note.text}')
        return '\n'.join(game_notes)

    def get_achievements_field(self):
        achievements = []
        for achievement in self.game_summary.achievements:
            achievements.append(f':star: {achievement.user_name} unlocked **{achievement.achievement_name}**\n*{achievement.achievment_description}*')
        return '\n\n'.join(achievements)