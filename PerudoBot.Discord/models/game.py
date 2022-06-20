import utils

class GameSummary():
    def __init__(self, json: dict) -> None:
        self.game_id = json.get('gameId')
        self.betting_changes : list[PlayerPointsChange] = parse_points_changes(json.get('betPointsChanges'))
        self.elo_changes : list[PlayerEloChange] = parse_elo_changes(json.get('eloChanges'))
        self.notes : list[GameNote] = parse_game_notes(json.get('notes'))
        self.achievements = utils.parse_user_achievements(json.get('achievements'))

class PlayerEloChange():
    def __init__(self, json: dict) -> None:
        self.name = json.get('name')
        self.starting_elo = json.get('startingElo')
        self.final_elo = json.get('finalElo')
        pass

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

def parse_elo_changes(json) -> list[PlayerEloChange]:
    elo_changes = []
    for elo_change in json:
        elo_changes.append(PlayerEloChange(elo_change))
    return elo_changes

def parse_points_changes(json) -> list[PlayerPointsChange]:
    points_changes = []
    for points_change in json:
        points_changes.append(PlayerPointsChange(points_change))
    return points_changes

def parse_game_notes(json) -> list[GameNote]:
    game_notes = []
    for note in json:
        game_notes.append(GameNote(note))
    return game_notes