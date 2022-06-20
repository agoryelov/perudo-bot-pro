import json
from .player import Player
from .action import Bid, Bet, Bid, Liar
import utils

class Round():
    def __init__(self, json: dict):
        self.players : dict[int, Player] = utils.parse_players(json.get('players'))
        self.discord_players : dict[int, Player] = utils.parse_discord_players(json.get('players'))
        self.bets : list[Bet] = utils.parse_bets(json.get('bets'))
        self.bids : list[Bid] = utils.parse_bids(json.get('bids'))
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
        self.any_bids : bool = len(self.bids) > 0
    
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
        message['action'] = utils.bid_to_action_index(self.latest_bid.quantity, self.latest_bid.pips)

        return json.dumps(message)

class RoundSummary():
    def __init__(self, json: dict):
        self.round = Round(json.get('round'))
        self.achievements = utils.parse_user_achievements(json.get('achievements'))
        