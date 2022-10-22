import discord

from models import AuctionSummary

class AuctionSummaryEmbed(discord.Embed):
    def __init__(self, auction: AuctionSummary):
        super().__init__()
        self.description = f':trophy: **{auction.winner.name}** aquired {auction.item.name} for `{auction.final_price} pts`'