import utils

class GameLog():
    def __init__(self, json: dict):
        self.recent_games: list[LogEntry] = utils.parse_list(json["logEntries"], LogEntry)

class LogEntry():
    def __init__(self, json: dict):
        self.id = json['gameId']
        self.state = json['gameState']
        self.date = json['gameDate']