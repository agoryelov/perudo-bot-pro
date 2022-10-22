import utils

class LadderInfo():
    def __init__(self, json: dict) -> None:
        self.entries: list[LadderEntry] = utils.parse_list(json.get('ladderEntries'), LadderEntry)

class LadderEntry():
    def __init__(self, json: dict) -> None:
        self.name = json.get('name')
        self.elo = json.get('elo')
        self.points = json.get('points')
        self.achievement_score = json.get('achievementScore')
        self.games_played = json.get('gamesPlayed')
