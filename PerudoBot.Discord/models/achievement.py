import utils

class Achievement():
    def __init__(self, json: dict):
        self.name = json['name']
        self.description = json['description']
        self.score = json['score']

class AchievementDetails(Achievement):
    def __init__(self, json: dict):
        super().__init__(json)
        self.unlocked_by : list[str] = json['unlockedBy']
        self.unlocks = json['unlocksCount']
        self.is_unlocked = self.unlocks > 0

class UserAchievement(Achievement):
    def __init__(self, json: dict):
        super().__init__(json)
        self.username = json['username']
        self.date_unlocked = utils.parse_date(json['dateUnlocked'])
