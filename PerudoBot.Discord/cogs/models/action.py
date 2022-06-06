class Action():
    def __init__(self, json: dict):
        if json is None: return
        self.player_id = json.get('playerId')
        self.action_type = json.get('actionType')

class Bid(Action):
    def __init__(self, json: dict):
        super().__init__(json)
        self.quantity = json.get('quantity')
        self.pips = json.get('pips')

class Liar(Action):
    def __init__(self, json: dict):
        super().__init__(json)
        self.target_bid : Bid = Bid(json.get('targetBid'))
        self.lives_lost = json.get('livesLost')
        self.losing_player_id = json.get('losingPlayerId')
        self.is_successful = json.get('isSuccessful')
        self.actual_quantity = json.get('actualQuantity')

class Bet(Action):
    def __init__(self, json: dict) -> None:
        super().__init__(json)
        self.target_bid : Bid = Bid(json.get('targetBid'))
        self.bet_type = json.get('betType')
        self.bet_amount = json.get('betAmount')
        self.is_successful = json.get('isSuccessful')
        self.bet_odds = json.get('betOdds')