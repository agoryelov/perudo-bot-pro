import discord
from cogs.models.round import Round, RoundSummary
from cogs.utils.helpers import get_emoji

class RoundSummaryEmbed(discord.Embed):
    def __init__(self, round_summary: RoundSummary):
        super().__init__(title=f'Round {round_summary.round.round_number} Summary')
        self.round = round_summary.round
        self.achievements = round_summary.achievements
        self.all_dice = self.get_all_dice()

        if len(self.round.bets) > 0:
            self.add_field(name='Bet Results', value=self.get_bet_results_field(), inline=False)

        self.add_field(name='Players', value=self.get_player_dice_field())
        self.add_field(name='Dice', value=self.get_dice_counts_field())
        self.add_field(name='Totals', value=self.get_dice_totals_field())

        if len(self.achievements) > 0:
            self.add_field(name='Achievements', value=self.get_achievements_field(), inline=False)

    def get_all_dice(self) -> list[int]:
        all_dice = []
        for player in self.round.players.values():
            all_dice.extend(player.dice)
        return all_dice

    def get_bet_results_field(self):
        bet_results = []
        for bet in self.round.bets:
            bet_player = self.round.players[bet.player_id]
            points_delta = round(bet.bet_amount * bet.bet_odds) if bet.is_successful else bet.bet_amount
            wins_or_loses = "wins" if bet.is_successful else "loses"
            odds = f'*({round(bet.bet_odds, 1)})*' if bet.is_successful else ''
            bet_type_text = 'exact' if bet.bet_type == 0 else 'liar'
            bet_results.append(f':dollar: {bet_player.name} **{wins_or_loses} {points_delta}** {odds} points betting {bet_type_text} on `{bet.target_bid.quantity}` ˣ {get_emoji(bet.target_bid.pips)}')
        return '\n'.join(bet_results)

    def get_player_dice_field(self):
        player_dice = []
        for player in self.round.players.values():
            if len(player.dice) > 0:
                player_dice.append(f'`{len(player.dice)}` {player.name} `{player.points} pts` \u200b \u1CBC{" ".join(get_emoji(x) for x in player.dice)}')
        return '\n'.join(player_dice)
    
    def get_dice_counts_field(self):
        dice_counts = []
        for face in range(1, 7):
            count = self.all_dice.count(face)
            dice_counts.append(f'`{count}` ˣ {get_emoji(face)}')
        return '\n'.join(dice_counts)

    def get_dice_totals_field(self):
        ones_count = self.all_dice.count(1)
        dice_totals = [f'`{ones_count}` ˣ :one:']
        for face in range(2, 7):
            count = self.all_dice.count(face) + ones_count
            dice_totals.append(f'`{count}` ˣ {get_emoji(face)}')
        return '\n'.join(dice_totals)

    def get_achievements_field(self):
        achievements = []
        for achievement in self.achievements:
            achievements.append(f':star: {achievement.user_name} unlocked **{achievement.achievement_name}**\n*{achievement.achievment_description}*')
        return '\n\n'.join(achievements)
