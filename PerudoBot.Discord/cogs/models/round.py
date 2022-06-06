from cogs.models.action import Bet, Bid, Liar
from cogs.models.player import Player
from cogs.models.shared import parse_achievements, parse_bets, parse_players

class Round():
    def __init__(self, json: dict):
        self.players : dict[int, Player] = parse_players(json.get('players'))
        self.bets : list[Bet] = parse_bets(json.get('bets'))
        self.action_player_id : int = json.get('activePlayerId')
        self.round_number : int = json.get('roundNumber')
        self.round_type : str = json.get('roundType')
        self.total_dice : int = json.get('totalDiceCount')
        self.active_player_count: int = json.get('activePlayerCount')
        
        latest_bid = json.get('latestBid')
        self.latest_bid : Bid = Bid(latest_bid) if latest_bid is not None else None

        liar = json.get('liar')
        self.liar : Liar = Liar(liar) if liar is not None else None
        
        self.is_final : bool = self.active_player_count == 1
        self.any_eliminated : bool = self.active_player_count < len(self.players)
        self.any_bets : bool = len(self.bets) > 0

        # self.actions : list[Action] = parse_actions(json.get('actions'))

class RoundSummary():
    def __init__(self, json: dict):
        self.round = Round(json.get('round'))
        self.achievements = parse_achievements(json.get('achievements'))
        