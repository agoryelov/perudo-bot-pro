from typing import Union
import discord
from models import UserProfile, UserGame
from models.achievement import UserAchievement

class UserProfileEmbed(discord.Embed):
    def __init__(self, profile: UserProfile, user: Union[discord.User, discord.Member]):
        super().__init__()
        self.set_author(name=profile.name, icon_url=user.avatar.url)
        self.set_thumbnail(url=user.avatar.url)
        self.add_field(name="Elo Rating", value=f":game_die: `{profile.elo}` *(#{profile.elo_rank})*")
        self.add_field(name="Points", value=f":dollar: `{profile.points}` *(#{profile.points_rank})*")
        self.add_field(name="Achievement Score", value=f":star: `{profile.score}` *(#{profile.score_rank})*", inline=False)
        self.add_field(name="Recent Games (5)", value=self.get_recent_games(profile.recent_games), inline=False)
        if len(profile.recent_achievements) > 0:
            self.add_field(name="Recent Achievements", value=self.get_recent_achievements(profile.recent_achievements))

    def get_recent_games(self, games: list[UserGame]):
        return f'Average Place: `{average_place(games)}` \nNet Points: `{net_points(games)} pts`'

    def get_recent_achievements(self, achievements: list[UserAchievement]):
        output = []
        for achievement in achievements:
            formatted_date = achievement.date_unlocked.strftime('%Y-%m-%d')
            output.append(f':star: `{achievement.score}` **{achievement.name}** @ `{formatted_date}`')
        return '\n'.join(output)

def average_place(games: list[UserGame]):
    if len(games) <= 0: return 'N/A'
    total = 0
    for game in games:
        scaled_place = (game.placing - 1) / game.player_count
        total += scaled_place * 100
    return f'Top {round(total / len(games))}%'

def net_points(games: list[UserGame]):
    if len(games) <= 0: return 'N/A'
    total = 0
    for game in games:
        total += game.net_points
    return f'{total:+}'