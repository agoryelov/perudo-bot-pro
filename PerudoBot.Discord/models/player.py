class Player():
    def __init__(self, json: dict):
        self.player_id : int = json.get('playerId')
        self.discord_id : int = json.get('discordId')
        self.is_bot : bool = json.get('isBot')
        self.name : str = json.get('name')
        self.dice : list[int] = json.get('dice')
        self.lives : int = json.get('lives')
        self.points : int = json.get('points')
        self.is_eliminated : bool = self.lives <= 0