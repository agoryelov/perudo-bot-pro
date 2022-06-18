from .player import Player
import utils

class GameSetup():
    def __init__(self, json: dict):
        self.players : dict[int, Player] = utils.parse_players(json.get('players'))
        self.discord_players : dict[int, Player] = utils.parse_discord_players(json.get('players'))
        self.round_type : str = utils.parse_round_type(json.get('defaultRoundType'))
        self.game_id : int = json.get('gameId')