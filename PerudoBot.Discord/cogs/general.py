from discord.ext import commands

from game import GameClient
from models import LadderInfo
from utils import GameActionError
from views import LadderInfoEmbed, LadderInfoView

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


async def setup(bot: commands.Bot):
    await bot.add_cog(General(bot))