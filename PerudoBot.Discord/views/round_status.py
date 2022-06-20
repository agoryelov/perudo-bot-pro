import discord
from discord import SelectOption
from models import Round
from utils import get_emoji, get_next_bid, next_up_message, SYM_X
from utils.exceptions import GameActionError

from typing import TYPE_CHECKING

from .round_summary import RoundSummaryEmbed
from .game_summary import GameSummaryEmbed

if TYPE_CHECKING:
    from game import GameDriver

class BidButton(discord.ui.Button['RoundView']):
    def __init__(self, quantity: int, pips: int, row: int):
        super().__init__(style=discord.ButtonStyle.grey, label=f'{quantity} {SYM_X} {pips}', row=row)
        self.quantity = quantity
        self.pips = pips
    
    async def callback(self, interaction: discord.Interaction):
        game_driver = self.view.game_driver
        try:
            round = await game_driver.bid_action(interaction.user.id, self.quantity, self.pips)
            await game_driver.update_round_message(round, interaction.response.edit_message)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class LiarButton(discord.ui.Button['RoundView']):
    def __init__(self, row: int):
        super().__init__(style=discord.ButtonStyle.red, label='Liar', row=row)
    
    async def callback(self, interaction: discord.Interaction):
        game_driver = self.view.game_driver
        try:
            round_summary = await game_driver.liar_action(interaction.user.id)
            await interaction.response.edit_message(view=None)
            await game_driver.send_liar_result(round_summary)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)
            return

        await game_driver.send_delayed(embed=RoundSummaryEmbed(round_summary))

        if game_driver.ended:
            game_summary = await game_driver.end_game()
            await game_driver.send_delayed(embed=GameSummaryEmbed(game_summary), delay=2)
        else:
            await game_driver.start_round()

class RoundView(discord.ui.View):
    def __init__(self, r: Round, game_driver: 'GameDriver'):
        super().__init__(timeout=1200)
        self.round = r
        self.game_driver = game_driver

        if r.latest_bid is None:
            starting_quantity = (r.total_dice // 3) or 1
            bid = starting_quantity, 2
        else: 
            bid = get_next_bid(r.latest_bid.quantity, r.latest_bid.pips)

        for i in range(7):
            self.add_item(BidButton(bid[0], bid[1], i // 4))
            bid = get_next_bid(bid[0], bid[1])
        
        self.add_item(LiarButton(1))
    
    @discord.ui.select(placeholder="Place bet...", options=[ 
        SelectOption(label='Bet Liar +10%', value='liar 0.10'), 
        SelectOption(label='Bet Liar +50%', value='liar 0.50'),
        SelectOption(label='Bet Exact +10%', value='exact 0.10'), 
        SelectOption(label='Bet Exact +50%', value='exact 0.50'), 
        ], row=2)
    async def bet(self, interaction: discord.Interaction, select: discord.ui.Select):
        bet_type, percent = select.values[0].split()

        player = self.game_driver.discord_players[interaction.user.id]
        bet_amount = int(player.points * float(percent))

        try:
            round = await self.game_driver.bet_action(interaction.user.id, bet_amount, bet_type)
            await self.game_driver.update_round_message(round, interaction.response.edit_message)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class RoundEmbed(discord.Embed):
    def __init__(self, r: Round):
        super().__init__(title=f'Round {r.round_number}')
        self.round = r
        self.add_field(name='Players', value=self.get_players_field())

        if self.round.any_eliminated:
            self.add_field(name="Eliminated", value=self.get_eliminated_field())

        if self.round.any_bets:
            self.add_field(name="Bets", value=self.get_bets_field(), inline=False)

        if self.round.any_bids:
            self.add_field(name="Game Log", value=self.get_gamelog_field(), inline=False)

        self.add_field(name="\u200b", value=next_up_message(r), inline=False)

        self.set_footer(text=f"Quick maths: {r.total_dice}/3 = {round(r.total_dice / 3.0, 2)}")

    def get_players_field(self):
        active_players = []
        for player in self.round.players.values():
            if player.lives > 0:
                active_players.append(f'`{len(player.dice)}` {player.name} `{player.points} pts`')
        return '\n'.join(active_players)
    
    def get_eliminated_field(self):
        eliminated = []
        for player in self.round.players.values():
            if player.lives == 0:
                eliminated.append(f':coffin: {player.name} `{player.points} pts`')
        if len(eliminated) == 0: return 'None'
        return '\n'.join(eliminated)

    def get_bets_field(self):
        bets = []
        for bet in self.round.bets:
            bet_player = self.round.players[bet.player_id]
            bet_type_text = 'exact' if bet.bet_type == 0 else 'a lie'
            bets.append(f':dollar: {bet_player.name} bets {bet.bet_amount} that `{bet.target_bid.quantity}` {SYM_X} {get_emoji(bet.target_bid.pips)} is {bet_type_text}')
        if len(bets) == 0: return 'None'
        return '\n'.join(bets)

    def get_gamelog_field(self):
        logs = []
        for bid in self.round.bids:
            time = bid.date_created.strftime('%H:%M')
            player = self.round.players[bid.player_id]
            logs.append(f'`{time}`: {player.name} bids `{bid.quantity}` {SYM_X} {get_emoji(bid.pips)}')
        if len(logs) == 0: return 'None'
        return '\n'.join(logs)