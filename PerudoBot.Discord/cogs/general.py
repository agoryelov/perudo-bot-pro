import discord
from discord.ext import commands
from discord import app_commands

class General(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    @commands.hybrid_command(name="ping", description="To check latency of bot.", help="To check latency of bot.")
    async def ping(self, ctx: commands.Context):
        await ctx.send(f'Pong! `{round(self.bot.latency * 1000)} ms`')

    @app_commands.command(name="bing")
    async def bing(self, interaction: discord.Interaction):
        await interaction.response.send_message(f'Bong! `{round(self.bot.latency * 1000)} ms`')

    @commands.hybrid_command(name='hello', description="To greet the bot and get greeted back.", help="Gives bot a greeting.")
    async def hello(self, ctx):
        await ctx.send(f'Hello {ctx.author.mention}, Hope you are keeping well!')


async def setup(bot: commands.Bot):
    await bot.add_cog(General(bot))