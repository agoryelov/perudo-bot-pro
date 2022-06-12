import json
from models.action import Bet, Bid, Liar
from models.player import Player
from utils.parsers import parse_achievements, parse_bets, parse_players
from utils.helpers import bid_to_action_index

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
    
    def bot_message(self) -> str:
        message = { 
            'round': self.round_number, 
            'gameDice': self.total_dice,
            'nextPlayer' : self.players[self.action_player_id].discord_id
        }
        
        if self.latest_bid is None:
            return json.dumps(message)

        current_player = self.players[self.latest_bid.player_id]
        message['currentPlayer'] = current_player.discord_id
        message['playerDice'] = len(current_player.dice)
        message['action'] = bid_to_action_index(self.latest_bid.quantity, self.latest_bid.pips)

        return json.dumps(message)

class RoundSummary():
    def __init__(self, json: dict):
        self.round = Round(json.get('round'))
        self.achievements = parse_achievements(json.get('achievements'))
        