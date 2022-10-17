import discord
from discord.ext import commands

from game import GameClient
from models import LadderInfo, UserProfile, UserInventory
from utils import RattleType, GameActionError, parse_achievement_details, parse_user_achievements, is_url_image
from views import LadderInfoEmbed, LadderInfoView, UserAchievementsEmbed, AchievementSource, PagedView, UserProfileEmbed, UserInventoryEmbed, UserInventoryView

class General(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    @commands.hybrid_command(name="ping", description="To check latency of bot.", help="To check latency of bot.")
    async def ping(self, ctx: commands.Context):
        await ctx.send(f'Pong! `{round(self.bot.latency * 1000)} ms`')
    
    @commands.hybrid_command(name="rattle", description="Set your rattle", help="Set your rattle")
    async def rattle(self, ctx: commands.Context, type: RattleType, rattle: str):
        if ctx.interaction is None:
            await ctx.reply('Use the slash command to set your rattle')
            return

        if not is_url_image(rattle):
            await ctx.reply('Rattle must be a direct link to an image or gif', ephemeral=True)
            return
        
        try:
            GameClient.update_rattle(ctx.author.id, rattle, type)
            await ctx.send(f'{type.name} rattle set.', ephemeral=True)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)
    
    @commands.hybrid_command(name="profile", description="Check profile of a user", help="Check profile of a user")
    async def profile(self, ctx: commands.Context, member: discord.Member=None):
        if member is None: member = ctx.author
        await ctx.defer()
        try:
            profile_data = GameClient.get_user_profile(member.id)
            profile_embed = UserProfileEmbed(UserProfile(profile_data), member)
            await ctx.send(embed=profile_embed)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)
    
    @commands.hybrid_command(name="inventory", description="Manage your inventory", help="Manage your inventory")
    async def inventory(self, ctx: commands.Context):
        await ctx.defer(ephemeral=True)
        try:
            inventory_data = GameClient.get_user_inventory(ctx.author.id)
            user_inventory = UserInventory(inventory_data)
            if ctx.interaction is None:
                await ctx.send(embed=UserInventoryEmbed(user_inventory))
            else:
                await ctx.send(embed=UserInventoryEmbed(user_inventory), view=UserInventoryView(user_inventory), ephemeral=True)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)
    
    @commands.hybrid_command(name="ladder", description="Show current ladder standings", help="Show current ladder standings")
    async def ladder(self, ctx: commands.Context):
        try:
            ladder_data = GameClient.get_ladder_info()
            ladder_info = LadderInfo(ladder_data)
            ladder_view = LadderInfoView(ladder_info.entries)
            ladder_view.message = await ctx.send(view=ladder_view, embed=LadderInfoEmbed(ladder_info.entries, 'Points'))
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
            achivements = parse_achievement_details(achievement_data)
            source = AchievementSource(achivements)
            paged_view = PagedView(source)
            paged_view.message = await ctx.send(view=paged_view, embed=source.page(1))
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

async def setup(bot: commands.Bot):
    await bot.add_cog(General(bot))