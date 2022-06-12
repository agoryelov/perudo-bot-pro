from models.player import Player
from utils.parsers import parse_players, parse_round_type

class GameSetup():
    def __init__(self, json: dict):
        self.players : dict[int, Player] = parse_players(json.get('players'))
        self.round_type : str = parse_round_type(json.get('defaultRoundType'))
        self.game_id : int = json.get('gameId')