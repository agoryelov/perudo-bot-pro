import discord
import utils

class PlayerRattle():
    def __init__(self, json: dict):
        self.rattle_type = json.get('rattleType')
        self.rattle_content_type = json.get('rattleContentType')
        self.content = json.get('content')
    
class Player():
    def __init__(self, json: dict):
        self.player_id: int = json.get('playerId')
        self.discord_id: int = json.get('discordId')
        self.is_bot: bool = json.get('isBot')
        self.name: str = json.get('name')
        self.dice: list[int] = json.get('dice')
        self.lives: int = json.get('lives')
        self.points: int = json.get('points')
        self.equipped_dice: str = json.get('equippedDice')
        self.is_eliminated: bool = self.lives <= 0

        if self.is_bot: self.avatar = ':robot:'
        else: self.avatar = utils.player_avatar(self.player_id)

        self._rattles: list[PlayerRattle] = utils.parse_list(json.get('rattles'), PlayerRattle)

    def rattle(self, type, content_type = 0) -> PlayerRattle:
        return discord.utils.get(self._rattles, rattle_type=type, rattle_content_type=content_type)
