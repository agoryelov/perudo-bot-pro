import discord
from discord.ext import commands
from dotenv import load_dotenv
from os import getenv

from services import PerudoContext, AuctionService, GameService
from utils import MessageType

load_dotenv()

OWNER_ID = getenv('OWNER_ID')
SYNC_GUILD = discord.Object(id=getenv('GUILD_ID'))
BOT_PREFIX = getenv('BOT_PREFIX', '!')

class PerudoChannel():
    def __init__(self, ctx: PerudoContext):
        self.channel_game = GameService(ctx)
        self.channel_auction = AuctionService(ctx)

        self._channel_messages: dict[MessageType, discord.Message] = {}

    def get_message(self, type: MessageType) -> discord.Message:
        return self._channel_messages.get(type)
    
    def set_message(self, type: MessageType, message: discord.Message):
        self._channel_messages[type] = message
    
class PerudoBot(commands.Bot):
    def __init__(self):
        super().__init__(command_prefix=BOT_PREFIX, intents=discord.Intents.all())

        self.owner_id = OWNER_ID
        self.extns = ['cogs.general', 'cogs.perudo']

        self._perudo_channels: dict[int, PerudoChannel] = {}

    def perudo_channel(self, ctx: PerudoContext):
        if ctx.channel.id not in self._perudo_channels:
            self._perudo_channels[ctx.channel.id] = PerudoChannel(ctx)
        return self._perudo_channels[ctx.channel.id]
    
    def get_message(self, ctx, type: MessageType):
        perudo_channel = self.perudo_channel(ctx)
        return perudo_channel.get_message(type)

    def set_message(self, ctx, type: MessageType, message: discord.Message):
        perudo_channel = self.perudo_channel(ctx)
        perudo_channel.set_message(type, message)
    
    def channel_game(self, ctx: PerudoContext):
        return self.perudo_channel(ctx).channel_game

    def channel_auction(self, ctx: PerudoContext):
        return self.perudo_channel(ctx).channel_auction

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