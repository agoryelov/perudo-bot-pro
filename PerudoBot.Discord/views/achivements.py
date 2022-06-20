import discord

from models import Achievement, UserAchievement
from utils import EmbedColor

class AchievementsEmbed(discord.Embed):
    def __init__(self, achievements: list[Achievement]):
        super().__init__()
        self.color = EmbedColor.Yellow
        self.title = f'Achievements'

        for achievement in achievements:
            self.add_field(name=self.get_entry_name(achievement), value=self.get_entry_value(achievement), inline=False)
    
    def get_entry_name(self, achievement: Achievement):
        return f':star: {achievement.name}'

    def get_entry_value(self, achievement: Achievement):
        if not achievement.is_unlocked:
            return '`Achievement Locked`'
        formatted_date = achievement.date_unlocked.strftime('%Y-%m-%d %H:%M')
        unlocked_text = f'First unlocked by **{achievement.unlocked_by}** @ `{formatted_date}`'
        return f'*{achievement.description}*\n{unlocked_text}'

class UserAchievementsEmbed(discord.Embed):
    def __init__(self, achievements: list[UserAchievement]):
        super().__init__()
        self.color = EmbedColor.Yellow
        user_name = achievements[0].user_name
        self.title = f'Achievements: {user_name} ({len(achievements)})'

        for achievement in achievements:
            self.add_field(name=self.get_entry_name(achievement), value=self.get_entry_value(achievement), inline=False)
    
    def get_entry_name(self, achievement: UserAchievement):
        formatted_date = achievement.date_unlocked.strftime('%Y-%m-%d %H:%M')
        return f':star: {achievement.name} @ `{formatted_date}`'

    def get_entry_value(self, achievement: UserAchievement):
        return f'*{achievement.description}*'