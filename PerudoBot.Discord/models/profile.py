import utils

from models import UserAchievement

class UserProfile():
    def __init__(self, json: dict):
        self.name = json['name']
        self.discord_id = json['discordId']
        self.points = json['points']
        self.points_rank = json["pointsRank"]
        self.score = json['score']
        self.score_rank = json["scoreRank"]
        self.elo = json['elo']
        self.elo_rank = json["eloRank"]
        self.recent_games: list[UserGame] = utils.parse_list(json["recentGames"], UserGame)
        self.recent_achievements: list[UserAchievement] = utils.parse_list(json["recentAchievements"], UserGame)

class UserGame():
    def __init__(self, json: dict):
        self.placing = json['placing']
        self.net_points = json['netPoints']
        self.player_count = json['playerCount']