import asyncio
from dotenv import load_dotenv
from os import getenv

from bot import PerudoBot

load_dotenv()

DISCORD_TOKEN = getenv('DISCORD_TOKEN')

async def load(bot: PerudoBot):
    for ext in bot.extns:
        await bot.load_extension(ext)

async def main():
    async with PerudoBot() as bot:
        await bot.loop.create_task(load(bot))
        await bot.start(DISCORD_TOKEN)

asyncio.run(main())