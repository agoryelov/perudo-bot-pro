import asyncio
from code import interact
import discord
from os import getenv
from discord.ext import commands

from services import PerudoContext
from utils import parse_bid, GameActionError
from views import RoundSummaryEmbed, GameSetupEmbed, GameSetupView

from bot import PerudoBot

class Perudo(commands.Cog):
    def __init__(self, bot: commands.Bot):
        self.bot = bot

    @commands.hybrid_command(name="new", description="Create a new game", help="Create a new game")
    async def new(self, ctx: PerudoContext):
        if getenv('BOT_CHANNEL', None) is not None:
            ctx.game.bot_channel = await self.bot.fetch_channel(getenv('BOT_CHANNEL'))

        if ctx.game.in_setup or ctx.game.in_progress:
            await ctx.reply("Game already exists, use `/terminate` before starting a new game")
            return
        
        if ctx.interaction is not None: await ctx.interaction.response.defer()
        else: await ctx.message.delete()

        try:
            game_setup = await ctx.game.create_game()
            await ctx.send_setup_message(game_setup)

        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="bid", description="Place a bid", help="Place a bid", aliases=['b'])
    async def bid(self, ctx: PerudoContext, *, bid_text: str):
        parsed_bid = parse_bid(bid_text)
        if parsed_bid is None:
            await ctx.reply("Invalid bid input", ephemeral=True)
            return
        quantity, pips = parsed_bid

        is_slash = ctx.interaction is not None

        try:
            await ctx.defer(ephemeral=True)
            round = await ctx.game.bid_action(ctx.author.id, quantity, pips)

            if is_slash: await ctx.reply('Bid placed', ephemeral=True)
            else: await ctx.message.delete()

            await ctx.update_round_message(round)
            if ctx.game.has_bots: await ctx.game.send_bot_updates(round)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="liar", description="Call liar", help="Call liar")
    async def liar(self, ctx: PerudoContext):
        is_slash = ctx.interaction is not None

        try:
            await ctx.defer(ephemeral=True)
            round_summary = await ctx.game.liar_action(ctx.author.id)

            if is_slash: await ctx.reply('Liar called', ephemeral=True)
            else: await ctx.message.delete()
            
            await ctx.clear_active_view()
            await ctx.game.send_liar_result(round_summary)

        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)
            return

        await asyncio.sleep(1)
        await ctx.channel.send(embed=RoundSummaryEmbed(round_summary))
        await asyncio.sleep(1)

        if ctx.game.ended:
            await ctx.game.end_game()
        else:
            round = await ctx.game.start_round()
            if ctx.game.has_bots: await ctx.game.send_bot_updates(round)
            await ctx.send_round_message(round)

    @commands.hybrid_command(name="terminate", description="Terminate current game", help="Terminate current game")
    async def terminate(self, ctx: PerudoContext):        
        try:
            await ctx.game.terminate()
            await ctx.reply(f'Terminated game {ctx.game.game_id}')
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="add", description="Add player to the game", help="Add player to the game", aliases=['a'])
    async def add(self, ctx: PerudoContext, user: discord.Member = None):
        if user is None: user = ctx.author
        is_slash = ctx.interaction is not None

        if not ctx.game.in_setup:
            await ctx.reply("No game setup, make sure you are in the right channel", ephemeral=True)
            return
        
        try:
            game_setup = await ctx.game.add_player(user)

            if is_slash: await ctx.reply(f'Added {user.name} to the game', ephemeral=True)
            else: await ctx.message.delete()

            await ctx.update_setup_message(game_setup)
        except GameActionError as e:
            await ctx.reply(e.message, ephemeral=True)

    @commands.hybrid_command(name="voice", description="Ask the bot to join your voice channel", help="Ask the bot to join your voice channel")
    async def voice(self, ctx: PerudoContext):
        if ctx.author.voice is None:
            await ctx.reply('You are not in a voice channel', ephemeral=True)
            return

        voice_channel = ctx.author.voice.channel
        try:
            voice_client = await voice_channel.connect()
        except Exception as e:
            await ctx.reply(e, ephemeral=True)
            return
        
        ctx.game.voice_client = voice_client
        await ctx.reply(f'Joined voice channel {voice_channel.name}', ephemeral=True)
    
    @commands.hybrid_command(name="leave", description="Leave voice channel", help="Leave voice channel")
    async def leave(self, ctx: PerudoContext):
        if ctx.voice_client:
            await ctx.guild.voice_client.disconnect()
            await ctx.reply("I'm leaving the voice channel", ephemeral=True)
        else:
            await ctx.reply("I'm not in a voice channel", ephemeral=True)

async def setup(bot: PerudoBot):
    await bot.add_cog(Perudo(bot))