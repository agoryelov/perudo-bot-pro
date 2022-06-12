class LadderInfo():
    def __init__(self, json: dict) -> None:
        self.entries = parse_ladder_entries(json.get('ladderEntries'))


class LadderEntry():
    def __init__(self, json: dict) -> None:
        self.name = json.get('name')
        self.elo = json.get('elo')
        self.points = json.get('points')
        self.games_played = json.get('gamesPlayed')

def parse_ladder_entries(json) -> list[LadderEntry]:
    ladder_entries = []
    for entry in json:
        ladder_entries.append(LadderEntry(entry))
    return ladder_entries