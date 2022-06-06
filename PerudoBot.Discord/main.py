import asyncio
import discord
from discord.ext import commands
from discord.ext.commands import MissingPermissions, CommandNotFound

MY_GUILD = discord.Object(id=689504722163335196)

bot = commands.Bot(command_prefix='!', intents=discord.Intents.all())
bot.owner_id = 153573546751164416

@bot.command(name='sync_guild', help='To sync application commands for current guild.', hidden=True)
# Command to sync application commands for current guild.
@commands.has_permissions(administrator=True)
async def sync_guild(ctx):
    if ctx.author.id != bot.owner_id:
        await ctx.send('You are not my owner!')  # Raise PermissionError
        return
    op = await ctx.send('Reloading..')
    for i in bot.extns:
        await bot.reload_extension(i)
    await op.edit(content='Reloaded!')
    await op.edit(content='Syncing...')
    await bot.tree.sync(guild=ctx.guild)
    await op.edit(content='Reloaded and Synced!')

@bot.command(name='reload', help='To reload all cogs.', hidden=True)  # Command to reload all cogs
@commands.has_permissions(administrator=True)
async def reload(ctx):
    if ctx.author.id != bot.owner_id:
        await ctx.send('You are not my owner!')  # Raise PermissionError
        return
    for i in bot.extns:
        await bot.reload_extension(i)
    bot.help_command.cog = bot.cogs.get('General')
    await ctx.send('Reloaded!')

@bot.event
async def on_ready():
    print(f'{bot.user.name} has connected to Discord!')

@bot.event
async def setup_hook():
    bot.tree.copy_global_to(guild=MY_GUILD)
    await bot.tree.sync(guild=MY_GUILD)

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
        await bot.start('OTc5OTQ5MjAxNjk1ODU0NjMz.G1-dsB.gBh27vg_Yb6xPqYsuPtP6FGrSb22ApQNgjxSd8')

asyncio.run(main())