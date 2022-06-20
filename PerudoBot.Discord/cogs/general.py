import discord
from discord.ext import commands

from game import GameClient
from models import LadderInfo
from utils import GameActionError, parse_achievements, parse_user_achievements
from views import LadderInfoEmbed, LadderInfoView, UserAchievementsEmbed, AchievementSource, PagedView

class General(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    @commands.hybrid_command(name="ping", description="To check latency of bot.", help="To check latency of bot.")
    async def ping(self, ctx: commands.Context):
        await ctx.send(f'Pong! `{round(self.bot.latency * 1000)} ms`')

    @commands.hybrid_command(name="ladder", description="Show current ladder standings", help="Show current ladder standings")
    async def ladder(self, ctx: commands.Context):
        try:
            ladder_data = GameClient.get_ladder_info()
            ladder_info = LadderInfo(ladder_data)
            ladder_view = LadderInfoView(ladder_info.entries)
            ladder_view.message = await ctx.send(view=ladder_view, embed=LadderInfoEmbed(ladder_info.entries))
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="achievements", description="Achievement information", help="Achievement information")
    async def achievements(self, ctx: commands.Context, member: discord.Member=None):
        if member is not None:
            await self.achievements_user(ctx, member.id)
        else:
            await self.achievements_summary(ctx)

    async def achievements_user(self, ctx: commands.Context, discrod_id):
        try:
            achievement_data = GameClient.get_user_achievements(discrod_id)
            achivements = parse_user_achievements(achievement_data)
            await ctx.send(embed=UserAchievementsEmbed(achivements))
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    async def achievements_summary(self, ctx: commands.Context):
        try:
            achievement_data = GameClient.get_achievements()
            achivements = parse_achievements(achievement_data)
            source = AchievementSource(achivements)
            paged_view = PagedView(source)
            paged_view.message = await ctx.send(view=paged_view, embed=source.page(1))
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

async def setup(bot: commands.Bot):
    await bot.add_cog(General(bot))