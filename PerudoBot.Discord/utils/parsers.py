from models import Player, Bet, Bid, Achievement, UserAchievement

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

def parse_bids(json) -> list[Bid]:
    if json is None: return []
    bids = []
    for bid in json:
        bids.append(Bid(bid))
    return bids

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

def parse_user_achievements(json) -> list[UserAchievement]:
    if json is None: return []
    achievements = []
    for achievement in json:
        achievements.append(UserAchievement(achievement))
    return achievements