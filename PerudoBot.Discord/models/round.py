from models.achievement import UserAchievement
from .player import Player
from .action import Bid, Bet, Bid, Liar

import utils

class Round():
    def __init__(self, json: dict):
        self.players: dict[int, Player] = utils.parse_players(json.get('players'), Player)
        self.discord_players: dict[int, Player] = utils.parse_discord_players(json.get('players'))
        self.bets: list[Bet] = utils.parse_list(json.get('bets'), Bet)
        self.bids: list[Bid] = utils.parse_list(json.get('bids'), Bid)
        self.can_reverse: bool = json.get('canReverse')
        self.action_player_id: int = json.get('activePlayerId')
        self.round_number: int = json.get('roundNumber')
        self.round_type: str = json.get('roundType')
        self.total_dice: int = json.get('totalDiceCount')
        self.active_player_count: int = json.get('activePlayerCount')
        
        latest_bid = json.get('latestBid')
        self.latest_bid : Bid = Bid(latest_bid) if latest_bid is not None else None

        liar = json.get('liar')
        self.liar : Liar = Liar(liar) if liar is not None else None
    
    @property
    def is_final(self) -> bool:
        return self.active_player_count == 1
    
    @property
    def any_eliminated(self) -> bool:
        return self.active_player_count < len(self.players)
    
    @property
    def any_bets(self) -> bool:
        return len(self.bets) > 0
    
    @property
    def any_bids(self) -> bool:
        return len(self.bids) > 0

    @property
    def any_liar_bets(self) -> bool:
        for bet in self.bets:
            if bet.bet_type is utils.BetType.Liar: return True
        return False

class RoundSummary():
    def __init__(self, json: dict):
        self.round = Round(json.get('round'))
        self.achievements: list[UserAchievement] = utils.parse_list(json.get('achievements'), UserAchievement)
        