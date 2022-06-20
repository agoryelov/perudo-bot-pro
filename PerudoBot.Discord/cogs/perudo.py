import asyncio
from typing import Literal
from discord.ext import commands

from discord import TextChannel

from game import GameDriver
from utils import parse_bid, GameActionError
from views import GameSetupView, GameSetupEmbed, RoundSummaryEmbed, GameSummaryEmbed

class Perudo(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.game_drivers : dict[int, GameDriver] = {}

    def _get_channel_game(self, channel: TextChannel) -> GameDriver:
        if channel.id not in self.game_drivers:
            self.game_drivers[channel.id] = GameDriver(channel)
        
        return self.game_drivers.get(channel.id)

    @commands.hybrid_command(name="new", description="Create a new game", help="Create a new game")
    async def new(self, ctx: commands.Context):
        is_slash = ctx.interaction is not None

        game_driver = self._get_channel_game(ctx.channel)

        if game_driver.in_setup or game_driver.in_progress:
            await ctx.reply("Game already exists, use `/terminate` before starting a new game")
            return
        
        if not is_slash: await ctx.message.delete()

        game_setup = await game_driver.create_game()
        game_setup_view = GameSetupView(game_driver)
        game_setup_message = await ctx.send(view=game_setup_view, embed=GameSetupEmbed(game_setup))

        await game_setup_view.wait()

        if game_setup_view.timed_out:
            await game_setup_message.edit(content='Setup timed out', view=None, embed=None)
            return

        try:
            await game_driver.start_game()
            await game_driver.start_round()
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="bid", description="Place a bid", help="Place a bid", aliases=['b'])
    async def bid(self, ctx: commands.Context, *, bid_text: str):
        parsed_bid = parse_bid(bid_text)
        if parsed_bid is None:
            await ctx.reply("Invalid bid input", ephemeral=True)
            return
        quantity, pips = parsed_bid

        is_slash = ctx.interaction is not None
        game_driver = self._get_channel_game(ctx.channel)

        try:
            round = await game_driver.bid_action(ctx.author.id, quantity, pips)

            if is_slash: await ctx.reply('Bid placed', ephemeral=True)
            else: await ctx.message.delete()

            await game_driver.update_round_message(round)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="liar", description="Call liar", help="Call liar")
    async def liar(self, ctx: commands.Context):
        is_slash = ctx.interaction is not None
        game_driver = self._get_channel_game(ctx.channel)

        try:
            round_summary = await game_driver.liar_action(ctx.author.id)

            if is_slash: await ctx.reply('Liar called', ephemeral=True)
            else: await ctx.message.delete()

            await game_driver.send_liar_result(round_summary)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)
            return

        await asyncio.sleep(1)
        await ctx.channel.send(embed=RoundSummaryEmbed(round_summary))
        await asyncio.sleep(1)

        if game_driver.ended:
            game_summary = await game_driver.end_game()
            await ctx.channel.send(embed=GameSummaryEmbed(game_summary))
        else:
            await game_driver.start_round()
    
    @commands.hybrid_command(name="bet", description="Place a bet on the latest bid", help="Place a bet on the latest bid")
    async def bet(self, ctx: commands.Context, bet_amount: int, bet_type: Literal['liar', 'exact']):
        is_slash = ctx.interaction is not None
        game_driver = self._get_channel_game(ctx.channel)

        try:
            round = await game_driver.bet_action(ctx.author.id, bet_amount, bet_type)

            if is_slash: await ctx.reply('Bet placed', ephemeral=True)
            else: await ctx.message.delete()

            await game_driver.update_round_message(round)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="terminate", description="Terminate current game", help="Terminate current game")
    async def terminate(self, ctx: commands.Context):
        game_driver = self._get_channel_game(ctx.channel)
                
        try:
            await game_driver.terminate()
            await ctx.reply(f'Terminated game {game_driver.game_id}')
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

async def setup(bot: commands.Bot):
    await bot.add_cog(Perudo(bot))