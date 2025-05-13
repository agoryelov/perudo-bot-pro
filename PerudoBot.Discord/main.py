from dotenv import load_dotenv
from os import getenv

from bot import PerudoBot

load_dotenv()

DISCORD_TOKEN = getenv('DISCORD_TOKEN')

bot = PerudoBot()
bot.run(DISCORD_TOKEN)