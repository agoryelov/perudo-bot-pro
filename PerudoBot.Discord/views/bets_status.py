import typing
import discord
from models import Round
from utils import bet_emoji, min_bet, player_emote
from utils import GameActionError, BetType, SYM_X

if typing.TYPE_CHECKING:
    from services import PerudoContext

class BetButton(discord.ui.Button['BetsView']):
    def __init__(self, bet_type: BetType):
        super().__init__(style=discord.ButtonStyle.grey, emoji=bet_emoji(bet_type))
        self.bet_type = bet_type
    
    async def callback(self, interaction: discord.Interaction):
        game_service = self.view.ctx.game
        better = game_service.discord_players[interaction.user.id]
        existing_bet = discord.utils.get(game_service.round.bets, player_id=better.player_id)
        
        bet_amount = min_bet(self.bet_type) if existing_bet is None else existing_bet.bet_amount * 4
        target_id = game_service.round.latest_bid.id

        try:
            r = await game_service.bet_action(interaction.user.id, bet_amount, self.bet_type, target_id)
            await self.view.ctx.update_bets_message(r, interaction.response.edit_message)
        except GameActionError as e:
            await interaction.response.send_message(e.message, ephemeral=True)

class BetsView(discord.ui.View):
    def __init__(self, ctx: 'PerudoContext'):
        super().__init__(timeout=1200)
        self.ctx = ctx
        self.round = ctx.game.round

        self.add_item(BetButton(BetType.Liar))
        self.add_item(BetButton(BetType.Exact))
        self.add_item(BetButton(BetType.Peak))
        self.add_item(BetButton(BetType.Legit))

class BetsEmbed(discord.Embed):
    def __init__(self, r: Round):
        super().__init__()
        self.round = r
        self.add_field(name="Bets", value=self.get_bets_field(), inline=False)
        self.set_footer(text=f'Betting on: {r.latest_bid.quantity} x {r.latest_bid.pips}')

    def get_bets_field(self):
        bets = []
        for bet in self.round.bets:
            target_player = self.round.players[bet.target_bid.player_id]
            bet_player = self.round.players[bet.player_id]
            bets.append(f':dollar: {bet_player.name} bets `{bet.bet_amount}` on `{bet.target_bid.quantity}` {SYM_X} {player_emote(bet.target_bid.pips, target_player.equipped_dice)}')
        if len(bets) == 0: return 'No bets yet'
        return '\n'.join(bets)