import utils

class UserInventory():
    def __init__(self, json: dict):
        self.name = json['name']
        self.equipped_dice = json.get('equippedDice')
        self.dice_items = utils.parse_dice_items(json.get('diceItems'))

        if self.equipped_dice is not None:
            self.equipped_dice = DiceItem(self.equipped_dice)

class UserItem():
    def __init__(self, json: dict):
        self.item_id = json['itemId']
        self.item_name = json['itemName']
        self.item_type = json['itemType']

class DiceItem(UserItem):
    def __init__(self, json: dict):
        super().__init__(json)
        self.emotes = json['diceEmotes']
