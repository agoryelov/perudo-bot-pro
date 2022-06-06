class Achievement():
    def __init__(self, json: dict):
        self.user_name = json['userName']
        self.achievement_name = json['achievementName']
        self.achievment_description = json['achievementDescription']
        self.date_unlocked = json['dateUnlocked']