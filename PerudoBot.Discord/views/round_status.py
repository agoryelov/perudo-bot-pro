import discord
from models import Round
from utils import get_emoji

class RoundEmbed(discord.Embed):
    def __init__(self, round_data: Round):
        super().__init__(title=f'Round {round_data.round_number}')
        self.round = round_data
        self.add_field(name='Players', value=self.get_players_field())

        if self.round.any_eliminated:
            self.add_field(name="Eliminated", value=self.get_eliminated_field())

        if self.round.any_bets:
            self.add_field(name="Bets", value=self.get_bets_field(), inline=False)

        if self.round.any_bids:
            self.add_field(name="Game Log", value=self.get_gamelog_field(), inline=False)
        
        self.set_footer(text=f"Quick maths: {round_data.total_dice}/3 = {round(round_data.total_dice / 3.0, 2)}")

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
            bets.append(f':dollar: {bet_player.name} bets {bet.bet_amount} that `{bet.target_bid.quantity}` ˣ {get_emoji(bet.target_bid.pips)} is {bet_type_text}')
        if len(bets) == 0: return 'None'
        return '\n'.join(bets)

    def get_gamelog_field(self):
        logs = []
        for bid in self.round.bids:
            time = bid.date_created.strftime('%H:%M')
            player = self.round.players[bid.player_id]
            logs.append(f'`{time}`: {player.name} bids `{bid.quantity}` ˣ {get_emoji(bid.pips)}')
        if len(logs) == 0: return 'None'
        return '\n'.join(logs)
