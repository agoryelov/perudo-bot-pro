import utils

class UserAchievement():
    def __init__(self, json: dict):
        self.user_name = json['username']
        self.name = json['name']
        self.description = json['description']
        self.date_unlocked = utils.parse_date(json['dateUnlocked'])

class Achievement():
    def __init__(self, json: dict):
        self.name = json['name']
        self.description = json['description']
        self.unlocks = json['unlocksCount']
        self.is_unlocked = self.unlocks > 0

        if self.is_unlocked:
            self.unlocked_by = json['unlockedBy']
            self.date_unlocked =  utils.parse_date(json['dateUnlocked'])