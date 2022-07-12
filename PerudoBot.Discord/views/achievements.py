import math
import discord

from models import AchievementDetails, UserAchievement
from utils import EmbedColor
from .paginator import PageSource

class AchievementSource(PageSource):
    def __init__(self, achievements: list[AchievementDetails]):
        super().__init__()
        self.achievements = achievements
        self.items_per_page = 5
    
    @property
    def num_pages(self) -> int:
        return math.ceil(len(self.achievements) / self.items_per_page)

    def page(self, page_num: int) -> discord.Embed:        
        end_index = page_num * self.items_per_page
        start_index = end_index - self.items_per_page
        end_index = min(end_index, len(self.achievements))
        embed = AchievementsEmbed(self.achievements[start_index:end_index])
        embed.set_footer(text=f'Results {start_index + 1} - {end_index} of {len(self.achievements)}')
        return embed

class AchievementsEmbed(discord.Embed):
    def __init__(self, achievements: list[AchievementDetails]):
        super().__init__()
        self.color = EmbedColor.Yellow
        self.title = f'Achievements'

        for achievement in achievements:
            self.add_field(name=self.get_entry_name(achievement), value=self.get_entry_value(achievement), inline=False)
    
    def get_entry_name(self, achievement: AchievementDetails):
        return f':star: `{achievement.score}` {achievement.name}'

    def get_entry_value(self, achievement: AchievementDetails):
        return f'*{achievement.description}*\n{format_unlocked(achievement.unlocked_by)}'

class UserAchievementsEmbed(discord.Embed):
    def __init__(self, achievements: list[UserAchievement]):
        super().__init__()
        self.color = EmbedColor.Yellow
        user_name = achievements[0].username
        self.title = f'Achievements: {user_name} ({len(achievements)})'

        for achievement in achievements:
            self.add_field(name=self.get_entry_name(achievement), value=self.get_entry_value(achievement), inline=False)
    
    def get_entry_name(self, achievement: UserAchievement):
        formatted_date = achievement.date_unlocked.strftime('%Y-%m-%d')
        return f':star: `{achievement.score}` {achievement.name} @ `{formatted_date}`'

    def get_entry_value(self, achievement: UserAchievement):
        return f'*{achievement.description}*'


def format_unlocked(usernames: list[str], max_show = 3):
    if len(usernames) == 0: return "`Locked` :lock:"
    formatted = f"`Unlocked by`: {', '.join(usernames[:max_show])}"
    if len(usernames) > max_show:
        formatted += f", *+{len(usernames) - max_show} more*"
    return formatted
