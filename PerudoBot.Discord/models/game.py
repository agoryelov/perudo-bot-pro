from .achievement import UserAchievement
from .inventory import Item
import utils

class GameSummary():
    def __init__(self, json: dict) -> None:
        self.game_id = json.get('gameId')
        self.winning_player_id = json.get('winningPlayerId')
        self.betting_changes : list[PlayerPointsChange] = utils.parse_list(json.get('betPointsChanges'), PlayerPointsChange)
        self.elo_changes : list[PlayerEloChange] = utils.parse_list(json.get('eloChanges'), PlayerEloChange)
        self.notes : list[GameNote] = utils.parse_list(json.get('notes'), GameNote)
        self.achievements: list[UserAchievement] = utils.parse_list(json.get('achievements'), UserAchievement)
        self.item_drops: list[ItemDrop] = utils.parse_list(json.get('itemDrops'), ItemDrop)

class PlayerEloChange():
    def __init__(self, json: dict) -> None:
        self.name = json.get('name')
        self.starting_elo = json.get('startingElo')
        self.final_elo = json.get('finalElo')

class PlayerPointsChange():
    def __init__(self, json: dict) -> None:
        self.name = json.get('name')
        self.starting_points = json.get('startingPoints')
        self.final_points = json.get('finalPoints')

class GameNote():
    def __init__(self, json: dict) -> None:
        self.round_number = json.get('roundNumber')
        self.name = json.get('name')
        self.text = json.get('text')

class ItemDrop():
    def __init__(self, json: dict) -> None:
        self.item = Item(json.get('item'))
        self.name = json.get('name')
