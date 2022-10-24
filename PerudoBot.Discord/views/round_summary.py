import discord
from models import Liar, Player, RoundSummary
from utils import SYM_X, EmbedColor, bet_emoji, RattleType, format_points, player_emote, system_emote
from typing import Dict, List

class DefeatEmbed(discord.Embed):
    def __init__(self, liar: Liar, players: Dict[int, Player]):
        super().__init__()
        losing_player = players[liar.losing_player_id]
        winning_player = players[liar.winning_player_id]
        self.description = f":skull: **{losing_player.name}** was defeated by **{winning_player.name}**"
        self.color = EmbedColor.Red

        deathrattle = losing_player.rattle(RattleType.Death)
        if deathrattle is not None:
            self.set_image(url=deathrattle.content)

class DamageDealtEmbed(discord.Embed):
    def __init__(self, liar: Liar, players: Dict[int, Player]):
        super().__init__()
        losing_player = players[liar.losing_player_id]
        winning_player = players[liar.winning_player_id]
        self.description = f"**{winning_player.name}** does `{liar.lives_lost}` {SYM_X} :heart: damage to **{losing_player.name}**"
        self.color = EmbedColor.Red

class LiarCalledEmbed(discord.Embed):
    def __init__(self, liar: Liar, players: Dict[int, Player], show_actual = False):
        super().__init__()
        target_bid = liar.target_bid
        liar_player = players[liar.player_id]
        bid_player = players[target_bid.player_id]

        self.description = f"**{liar_player.name}** called liar on **{bid_player.name}**'s `{target_bid.quantity}` {SYM_X} {system_emote(target_bid.pips)}"
        if show_actual:
            self.description += f"\nThere was actually `{liar.actual_quantity}` {SYM_X} {system_emote(target_bid.pips)}"
        
        if show_actual and liar.actual_quantity == target_bid.quantity:
            winning_player = players[liar.winning_player_id]
            tauntrattle = winning_player.rattle(RattleType.Taunt)
            if tauntrattle is not None:
                self.set_image(url=tauntrattle.content)
        
        self.color = EmbedColor.Red
    
class RoundSummaryEmbed(discord.Embed):
    def __init__(self, round_summary: RoundSummary):
        super().__init__(title=f'Round {round_summary.round.round_number} Summary')
        self.round = round_summary.round
        self.achievements = round_summary.achievements
        self.all_dice = self.get_all_dice()
        self.description = self.get_player_dice_field()
        
        if len(self.round.bets) > 0:
            self.add_field(name='Bet Results', value=self.get_bet_results_field(), inline=False)

        self.add_field(name='Dice', value=self.get_dice_counts_field())
        self.add_field(name='Totals', value=self.get_dice_totals_field())

        if len(self.achievements) > 0:
            self.add_field(name='Achievements', value=self.get_achievements_field(), inline=False)

    def get_all_dice(self) -> List[int]:
        all_dice = []
        for player in self.round.players.values():
            all_dice.extend(player.dice)
        return all_dice

    def get_bet_results_field(self):
        bet_results = []
        for bet in self.round.bets:
            bet_player = self.round.players[bet.player_id]
            points_delta = round(bet.bet_amount * bet.bet_odds) - bet.bet_amount if bet.is_successful else bet.bet_amount
            wins_or_loses = "wins" if bet.is_successful else "loses"
            odds = f'*(x{round(bet.bet_odds - 1, 1):.1f})*' if bet.is_successful else ''
            bet_results.append(f':dollar: {bet_player.name} **{wins_or_loses} {points_delta}** {odds} points betting {bet_emoji(bet.bet_type)} on `{bet.target_bid.quantity}` {SYM_X} {system_emote(bet.target_bid.pips)}')
        return '\n'.join(bet_results)

    def get_player_dice_field(self, max_players = 15):
        player_dice = []
        for player in self.round.players.values():
            if len(player.dice) > 0:
                player_dice.append(f'`{len(player.dice)}` {player.name} {format_points(player.points)} \u200b{" ".join(player_emote(x, player.equipped_dice) for x in player.dice)}')

        player_count = len(player_dice)
        if player_count > max_players:
            player_dice = player_dice[:max_players]
            player_dice.append(f'*{player_count - max_players} more player(s) hidden...*')

        return '\n'.join(player_dice)
    
    def get_dice_counts_field(self):
        dice_counts = []
        for face in range(1, 7):
            count = self.all_dice.count(face)
            dice_counts.append(f'`{count}` {SYM_X} {system_emote(face)}')
        return '\n'.join(dice_counts)

    def get_dice_totals_field(self):
        ones_count = self.all_dice.count(1)
        dice_totals = [f'`{ones_count}` {SYM_X} {system_emote(1)}']
        for face in range(2, 7):
            count = self.all_dice.count(face) + ones_count
            dice_totals.append(f'`{count}` {SYM_X} {system_emote(face)}')
        return '\n'.join(dice_totals)

    def get_achievements_field(self):
        achievements = []
        for achievement in self.achievements:
            achievements.append(f':star: `{achievement.score}` {achievement.username} unlocked **{achievement.name}**\n*{achievement.description}*')
        return '\n\n'.join(achievements)
