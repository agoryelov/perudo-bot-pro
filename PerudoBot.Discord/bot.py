import discord
from discord.ext import commands
from dotenv import load_dotenv
from os import getenv

from services import PerudoContext, AuctionService, GameService

load_dotenv()

OWNER_ID = getenv('OWNER_ID')
SYNC_GUILD = discord.Object(id=getenv('GUILD_ID'))
BOT_PREFIX = getenv('BOT_PREFIX', '!')

class PerudoBot(commands.Bot):
    def __init__(self):
        super().__init__(command_prefix=BOT_PREFIX, intents=discord.Intents.all())

        self.owner_id = OWNER_ID
        self.extns = ['cogs.general', 'cogs.perudo']

        self.channel_games: dict[int, GameService] = {}
        self.channel_auctions: dict[int, AuctionService] = {}
        self.active_message: discord.Message

    async def clear_active_view(self, text = ''):
        await self.active_message.edit(content=text, view=None)
    
    def channel_game(self, ctx: PerudoContext):
        if ctx.channel.id not in self.channel_games:
            self.channel_games[ctx.channel.id] = GameService(ctx)
        return self.channel_games[ctx.channel.id]

    def channel_auction(self, ctx: PerudoContext):
        if ctx.channel.id not in self.channel_auctions:
            self.channel_auctions[ctx.channel.id] = AuctionService(ctx)
        return self.channel_auctions[ctx.channel.id]

    async def setup_hook(self):
        self.tree.copy_global_to(guild=SYNC_GUILD)
        await self.tree.sync(guild=SYNC_GUILD)

    async def on_ready(self):
        print(f'{self.user.name} has connected to Discord!')

    async def get_context(self, origin, /, *, cls=PerudoContext) -> PerudoContext:
        return await super().get_context(origin, cls=cls)
    
    async def process_commands(self, message):
        ctx = await self.get_context(message)
        await self.invoke(ctx)

    async def on_command_error(self, ctx: PerudoContext, error: commands.CommandError):
        if isinstance(error, commands.CommandInvokeError):
            original = error.original
            print('In %s:', ctx.command.qualified_name, exc_info=original)   
        else:
            print('In %s:', ctx.command, exc_info=error)