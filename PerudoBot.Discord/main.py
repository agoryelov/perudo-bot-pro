import asyncio
import discord
from discord.ext import commands
from dotenv import load_dotenv
from os import getenv

load_dotenv()

DISCORD_TOKEN = getenv('DISCORD_TOKEN')
OWNER_ID = getenv('OWNER_ID')
MY_GUILD = discord.Object(id=getenv('GUILD_ID'))
BOT_PREFIX = getenv('BOT_PREFIX', '!')

bot = commands.Bot(command_prefix=BOT_PREFIX, intents=discord.Intents.all())
bot.owner_id = int(OWNER_ID)

@bot.event
async def on_ready():
    print(f'{bot.user.name} has connected to Discord!')

@bot.event
async def setup_hook():
    bot.tree.copy_global_to(guild=MY_GUILD)
    await bot.tree.sync(guild=MY_GUILD)

@bot.event
async def on_message(message):
    ctx = await bot.get_context(message)
    await bot.invoke(ctx)
    
@bot.event
async def on_command_error(ctx: commands.Context, error: commands.CommandError):
    print(type(error))
    print(error)

bot.extns = ['cogs.general', 'cogs.perudo']
async def load():
    for ext in bot.extns:
        await bot.load_extension(ext)

async def main():
    async with bot:
        await bot.loop.create_task(load())
        await bot.start(DISCORD_TOKEN)

asyncio.run(main())