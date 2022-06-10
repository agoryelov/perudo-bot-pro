import asyncio
from typing import Literal
from discord.ext import commands
from cogs.models.game import GameSummary
from cogs.models.ladder_info import LadderInfo
from cogs.models.round import Round, RoundSummary
from cogs.models.setup import GameSetup
from cogs.utils.client import GameClient
from cogs.utils.helpers import get_emoji, get_mention, parse_bid
from cogs.views.game_setup import GameSetupEmbed, GameSetupView
from cogs.views.game_summary import GameSummaryEmbed
from cogs.views.ladder_info import LadderInfoEmbed
from cogs.views.round_status import RoundEmbed
from cogs.views.round_summary import RoundSummaryEmbed

class Perudo(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.game_channels : dict[int, GameClient] = {}

    @commands.hybrid_command(name="new", description="Create a new game", help="Create a new game")
    async def new(self, ctx: commands.Context):
        is_slash = ctx.interaction is not None

        game_client = self.game_channels.get(ctx.channel.id)
        if game_client is not None and game_client.game_in_progress():
            await ctx.reply("Game already in progress", ephemeral=True)
            return
        
        game_client = GameClient()
        response = game_client.create_game()
        if not response.is_success:
            await ctx.reply(response.error_message, ephemeral=True)
            return
        
        if not is_slash: await ctx.message.delete()

        game_setup = GameSetup(response.data)
        self.game_channels[ctx.channel.id] = game_client
        game_setup_view = GameSetupView(game_client)

        await ctx.send(view=game_setup_view, embed=GameSetupEmbed(game_setup))
        await game_setup_view.wait()

        game_client.round_message = await ctx.channel.send(content="Starting game...")
        
        await asyncio.sleep(0.5)

        game_client.start_game()
        await self.start_round(ctx, game_client, is_slash)

    @commands.hybrid_command(name="bid", description="Place a bid", help="Place a bid", aliases=['b'])
    async def bid(self, ctx: commands.Context, *, bid_text: str):
        is_slash = ctx.interaction is not None
        
        bid_text = parse_bid(bid_text)
        if bid_text is None:
            await ctx.reply("Invalid bid input", ephemeral=True)
            return
        else:
            quantity, pips = bid_text
        
        game_client = self.game_channels.get(ctx.channel.id)
        if game_client is None or not game_client.game_in_progress():
            await ctx.reply("No active game", ephemeral=True)
            return
        
        response = game_client.bid_action(ctx.author.id, quantity, pips)
        if not response.is_success:
            if is_slash: await ctx.reply(response.error_message, ephemeral=True)
            return
        
        round_state = Round(response.data)
        player = game_client.get_player(ctx.author.id)

        if not is_slash: await ctx.message.delete()
        next_player = f'<@!{round_state.players[round_state.action_player_id].discord_id}>'
        await ctx.send(f'{player.name} bids `{quantity}` ˣ {get_emoji(pips)}. {next_player} is up.')
    
    @commands.hybrid_command(name="liar", description="Call liar", help="Call liar")
    async def liar(self, ctx: commands.Context):
        is_slash = ctx.interaction is not None
        
        game_client = self.game_channels.get(ctx.channel.id)
        if game_client is None or not game_client.game_in_progress():
            await ctx.reply('No active game', ephemeral=True)
            return
        
        response = game_client.liar_action(ctx.author.id)
        if not response.is_success:
            if is_slash: await ctx.reply(response.error_message, ephemeral=True)
            return

        if not is_slash: await ctx.message.delete()

        round_summary = RoundSummary(response.data)
        round_state = round_summary.round
        player = game_client.get_player(ctx.author.id)

        await ctx.send(f'{player.name} called **liar** on `{round_state.latest_bid.quantity}` ˣ {get_emoji(round_state.latest_bid.pips)}.')

        await asyncio.sleep(2)
        
        liar_action = round_state.liar
        losing_player = round_state.players[liar_action.losing_player_id]
        
        life_or_lives = 'life' if liar_action.lives_lost == 1 else 'lives'
        await ctx.channel.send(f'There was actually `{round_state.liar.actual_quantity}` dice. :fire: {get_mention(losing_player.discord_id)} loses {liar_action.lives_lost} {life_or_lives} :fire:')

        if losing_player.lives <= 0:
            await ctx.channel.send(f':fire::skull::fire: {losing_player.name} was defeated :fire::skull::fire:')

        await asyncio.sleep(1)

        game_client.round_message = None
        await ctx.channel.send(embed=RoundSummaryEmbed(round_summary))

        if round_state.is_final:
            await asyncio.sleep(1)
            await self.end_game(ctx, game_client)
        else:
            await self.start_round(ctx, game_client, is_slash)
    
    @commands.hybrid_command(name="bet", description="Place a bet on the latest bid", help="Place a bet on the latest bid")
    async def bet(self, ctx: commands.Context, bet_amount: int, bet_type: Literal['liar', 'exact']):
        is_slash = ctx.interaction is not None

        game_client = self.game_channels.get(ctx.channel.id)
        if game_client is None or not game_client.game_in_progress():
            await ctx.reply('No active game', ephemeral=True)
            return

        response = game_client.bet_action(ctx.author.id, bet_amount, bet_type)
        if not response.is_success:
            if is_slash: await ctx.send(response.error_message, ephemeral=True)
            return

        if not is_slash: await ctx.message.delete()
        player = game_client.get_player(ctx.author.id)
        round_state = Round(response.data)
        bet_type_text = 'a lie' if bet_type == 'liar' else 'exact'
        await ctx.send(f':dollar: {player.name} bets {bet_amount} that `{round_state.latest_bid.quantity}` ˣ {get_emoji(round_state.latest_bid.pips)} is {bet_type_text}')
        await game_client.round_message.edit(embed=RoundEmbed(round_state))

    @commands.hybrid_command(name="terminate", description="Terminate current game", help="Terminate current game")
    async def terminate(self, ctx: commands.Context):
        game_client = self.game_channels.get(ctx.channel.id)
        if game_client is None or not game_client.game_in_progress():
            await ctx.reply('No active game', ephemeral=True)
            return
        
        response = game_client.terminate_game()
        if not response.is_success:
            await ctx.reply(response.error_message, ephemeral=True)
            return
        
        await ctx.reply(f'Terminated game {game_client.game_id}')

    @commands.hybrid_command(name="note", description="Add a note to a game", help="Add a note to a game")
    async def note(self, ctx: commands.Context, note_text: str):
        if ctx.interaction is None: return
        
        game_client = self.game_channels.get(ctx.channel.id)
        if game_client is None or not game_client.game_in_progress():
            await ctx.reply('No active game', ephemeral=True)
            return
        
        response = game_client.add_note(ctx.author.id, note_text)
        if not response.is_success:
            await ctx.reply(response.error_message, ephemeral=True)
            return
        
        await ctx.send(f'Added note: `{note_text}`', ephemeral=False)

    @commands.hybrid_command(name="ladder", description="Show current ladder standings", help="Show current ladder standings")
    async def ladder(self, ctx: commands.Context):        
        response = GameClient.get_ladder_info()
        if not response.is_success:
            await ctx.reply(response.error_message, ephemeral=True)
            return

        ladder_info = LadderInfo(response.data)
        await ctx.send(embed=LadderInfoEmbed(ladder_info))


    async def end_game(self, ctx: commands.Context, game_client: GameClient):
        response = game_client.end_game()

        if not response.is_success:
            await ctx.reply(response.error_message, ephemeral=True)
            return
        
        game_summary = GameSummary(response.data)
        await ctx.channel.send(embed=GameSummaryEmbed(game_summary))
    
    async def start_round(self, ctx: commands.Context, game_client: GameClient, is_slash: bool):
        if game_client.round_message is None:
            game_client.round_message = await ctx.channel.send(content="Creating round...")
        else:
            await game_client.round_message.edit(content="Creating round...")

        await asyncio.sleep(1)

        response = game_client.start_round()
        if not response.is_success:
            if is_slash: await ctx.reply(response.error_message, ephemeral=True)
            return
        
        round_state = Round(response.data)
        await game_client.round_message.edit(content="", embed=RoundEmbed(round_state))
        await self.send_out_dice(ctx)

        next_player = f'<@!{round_state.players[round_state.action_player_id].discord_id}>'
        await ctx.channel.send(f'A new round has begun. {next_player} goes first.')
    
    async def send_out_dice(self, ctx: commands.Context):
        game_client = self.game_channels.get(ctx.channel.id)

        # all_dice = []
        # for player in game_client.players.values():
        #     all_dice.append(f'{player.name}=[{",".join(str(x) for x in player.dice)}]')
        # await ctx.channel.send(f'`{" ".join(all_dice)}`')

        for discord_id, player in game_client.players.items():
            if len(player.dice) > 0:
                member = ctx.guild.get_member(discord_id)
                await member.send(f'Your dice: {" ".join(get_emoji(x) for x in player.dice)}')

async def setup(bot: commands.Bot):
    await bot.add_cog(Perudo(bot))