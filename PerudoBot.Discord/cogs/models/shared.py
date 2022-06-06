from cogs.models.achievement import Achievement
from cogs.models.action import Action, Bet
from cogs.models.player import Player

def parse_players(json) -> dict[int, Player]:
    players = {}
    for player in json:
        players[player['playerId']] = Player(player)
    return players

def parse_bets(json) -> list[Bet]:
    if json is None: return []
    bets = []
    for bet in json:
        bets.append(Bet(bet))
    return bets

def parse_actions(json) -> list[Action]:
    if json is None: return []
    actions = []
    for action in json:
        actions.append(Action(action))
    return actions

def parse_round_type(type_id: int):
    if (type_id == 0): return 'Sudden Death'
    if (type_id == 1): return 'Reverse'
    return 'Default'

def parse_discord_players(json) -> dict[int, Player]:
    players = {}
    for player in json:
        players[player['discordId']] = Player(player)
    return players

def parse_achievements(json) -> list[Achievement]:
    if json is None: return []
    achievements = []
    for achievement in json:
        achievements.append(Achievement(achievement))
    return achievements
