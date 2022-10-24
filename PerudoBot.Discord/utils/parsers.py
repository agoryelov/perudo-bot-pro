from datetime import datetime
from models import Player

def parse_list(json, cls):
    if json is None: return []
    output = []
    for item in json:
        output.append(cls(item))
    return output

def parse_players(json, cls):
    players = {}
    for player in json:
        players[player['playerId']] = cls(player)
    return players

def parse_discord_players(json):
    players = {}
    for player in json:
        players[player['discordId']] = Player(player)
    return players

def parse_round_type(type_id: int):
    if (type_id == 0): return 'Sudden Death'
    if (type_id == 1): return 'Reverse'
    return 'Default'

def parse_date(date_string) -> datetime:
    return datetime.strptime(date_string, '%Y-%m-%dT%H:%M:%S%z')