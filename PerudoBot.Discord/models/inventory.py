import utils

class UserInventory():
    def __init__(self, json: dict):
        self.name = json['name']
        self.equipped_dice = json.get('equippedDice')
        self.dice_items: list[Item] = utils.parse_list(json.get('diceItems'), Item)

        if self.equipped_dice is not None:
            self.equipped_dice = Item(self.equipped_dice)

class Item():
    def __init__(self, json: dict):
        self.id = json['itemId']
        self.name = json['itemName']
        self.type = json['itemType']
        self.content = json['content']
        self.price = json['price']