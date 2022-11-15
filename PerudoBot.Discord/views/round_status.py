import asyncio
import typing
import discord
from models import Round
from utils import get_pip_quantity, next_up_message, deal_dice_message, bet_emoji, min_bet, format_points, player_emote
from utils import GameActionError, BetType, SYM_X, MessageType

from .round_summary import RoundSummaryEmbed

if typing.TYPE_CHECKING:
    from services import PerudoContext

class BidButton(discord.ui.Button['RoundView']):
    def __init__(self, quantity: int, pips: int, row: int, pip_used: bool):
        super().__init__(label=f'{quantity} {SYM_X} {pips}', row=row)
        self.quantity = quantity
        self.pips = pips

        if pip_used:
            self.style = discord.ButtonStyle.blurple
        else:
            self.style = discord.ButtonStyle.gray
    
    async def callback(self, interaction: discord.Interaction):
        game_service = self.view.ctx.game

        try:
            await interaction.response.defer()
            round = await game_service.bid_action(interaction.user.id, self.quantity, self.pips)
            await self.view.ctx.update_round_message(round)
            if game_service.has_bots: await game_service.send_bot_updates(round)

            if game_service.round.any_bets:
                await asyncio.sleep(3)
                round = await game_service.current_round()
                await self.view.ctx.update_bets_message(round)
        except GameActionError as e:
            if not interaction.user.bot: await interaction.user.send(f"`Bid Failed`: {e.message}")

class LiarButton(discord.ui.Button['RoundView']):
    def __init__(self, row: int):
        super().__init__(style=discord.ButtonStyle.red, label='Liar', row=row)
    
    async def callback(self, interaction: discord.Interaction):
        game_service = self.view.ctx.game

        try:
            await interaction.response.defer()
            round_summary = await game_service.liar_action(interaction.user.id)
            await self.view.ctx.clear_message(type=MessageType.Round)
            await self.view.ctx.clear_message(type=MessageType.Bets)
            await game_service.send_liar_result(round_summary)
        except GameActionError as e:
            if not interaction.user.bot: await interaction.user.send(f"`Liar Failed`: {e.message}")
            return

        await self.view.ctx.send_delayed(embed=RoundSummaryEmbed(round_summary))

        if game_service.ended:
            await game_service.end_game()
        else:
            round = await game_service.start_round()
            if game_service.has_bots: await game_service.send_bot_updates(round)
            await self.view.ctx.send_round_message(round)

class DiceButton(discord.ui.Button['RoundView']):
    def __init__(self, row: int,):
        super().__init__(style=discord.ButtonStyle.blurple, label=f'Dice', row=row)
    
    async def callback(self, interaction: discord.Interaction):
        discord_players = self.view.round.discord_players
        player = discord_players[interaction.user.id]
        if interaction.user.id in discord_players:
            player = discord_players[interaction.user.id]
            await interaction.response.send_message(deal_dice_message(player), ephemeral=True)
        else:
            await interaction.response.send_message("You are not in this game", ephemeral=True)

class ReverseButton(discord.ui.Button['RoundView']):
    def __init__(self, row: int, enabled=True):
        super().__init__(style=discord.ButtonStyle.blurple, emoji='🪃', row=row)
        self.disabled = not enabled
    
    async def callback(self, interaction: discord.Interaction):
        game_service = self.view.ctx.game

        try:
            r = await game_service.reverse_action(interaction.user.id)
            await self.view.ctx.update_round_message(r, interaction.response.edit_message)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class BetButton(discord.ui.Button['RoundView']):
    def __init__(self, bet_type: BetType, row: int, enabled=True):
        super().__init__(style=discord.ButtonStyle.grey, emoji=bet_emoji(bet_type), row=row)
        self.disabled = not enabled
        self.bet_type = bet_type
    
    async def callback(self, interaction: discord.Interaction):
        game_service = self.view.ctx.game 
        bet_amount = min_bet(self.bet_type)
        target_id = game_service.round.latest_bid.id

        try:
            r = await game_service.bet_action(interaction.user.id, bet_amount, self.bet_type, target_id)
            await self.view.ctx.update_round_message(r, interaction.response.edit_message)
            await self.view.ctx.send_bets_message(r)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class RoundView(discord.ui.View):
    def __init__(self, ctx: 'PerudoContext'):
        super().__init__(timeout=1200)
        self.ctx = ctx
        self.round = ctx.game.round
        
        if self.round.latest_bid is None:
            starting_quantity = round(self.round.total_dice / 3) - 1
            latest_bid = starting_quantity, 6
        else: 
            latest_bid = self.round.latest_bid.quantity, self.round.latest_bid.pips

        for i, pips in enumerate([2, 3, 4, 5, 6, 1]):
            quantity = get_pip_quantity(pips, latest_bid[0], latest_bid[1])
            pip_used = discord.utils.get(self.round.bids, pips=pips) is not None
            self.add_item(BidButton(quantity, pips, i // 3, pip_used))

        self.add_item(LiarButton(0))
        self.add_item(DiceButton(1))

        if not self.round.any_bets:
            self.add_item(BetButton(BetType.Liar, 2, enabled=self.round.any_bids))
            self.add_item(BetButton(BetType.Exact, 2, enabled=self.round.any_bids))
            self.add_item(BetButton(BetType.Peak, 2, enabled=self.round.any_bids))

            if self.round.any_bids:
                self.add_item(BetButton(BetType.Legit, 2, enabled=self.round.any_bids))
            else:
                self.add_item(ReverseButton(2, enabled=self.round.can_reverse))

class RoundEmbed(discord.Embed):
    def __init__(self, r: Round):
        super().__init__(title=f'Round {r.round_number}')
        self.round = r
        self.add_field(name='Players', value=self.get_players_field())

        if self.round.any_eliminated:
            self.add_field(name="Eliminated", value=self.get_eliminated_field())

        # if self.round.any_bets:
        #     self.add_field(name="Bets", value=self.get_bets_field(), inline=False)

        if self.round.any_bids:
            self.add_field(name="Game Log", value=self.get_gamelog_field(), inline=False)

        self.add_field(name="\u200b", value=next_up_message(r), inline=False)

        self.set_footer(text=f"Quick maths: {r.total_dice}/3 = {round(r.total_dice / 3.0, 2)}")

    def get_players_field(self):
        active_players = []
        for player in self.round.players.values():
            if player.lives > 0:
                active_players.append(f'`{len(player.dice)}` {player.avatar} {player.name} {format_points(player.points)}')
        return '\n'.join(active_players)
    
    def get_eliminated_field(self):
        eliminated = []
        for player in self.round.players.values():
            if player.lives == 0:
                eliminated.append(f':coffin: {player.name} {format_points(player.points)}')
        if len(eliminated) == 0: return 'None'
        return '\n'.join(eliminated)

    def get_bets_field(self):
        bets = []
        for bet in self.round.bets:
            target_player = self.round.players[bet.target_bid.player_id]
            bet_player = self.round.players[bet.player_id]
            bets.append(f':dollar: {bet_player.name} bets `{bet.bet_amount}` on `{bet.target_bid.quantity}` {SYM_X} {player_emote(bet.target_bid.pips, target_player.equipped_dice)}')
        if len(bets) == 0: return 'None'
        return '\n'.join(bets)

    def get_gamelog_field(self):
        logs = []
        for bid in self.round.bids:
            time = bid.date_created.strftime('%H:%M')
            player = self.round.players[bid.player_id]
            logs.append(f'`{time}`: {player.name} bids `{bid.quantity}` {SYM_X} {player_emote(bid.pips, player.equipped_dice)}')
        if len(logs) == 0: return 'None'
        return '\n'.join(logs)