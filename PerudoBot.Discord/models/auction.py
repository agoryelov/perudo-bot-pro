import random
import utils

from typing import Dict
from .inventory import Item

class Auction():
    def __init__(self, json: dict):
        self.id : int = json.get('auctionId')
        self.actions: list[AuctionAction] = utils.parse_list(json.get('actions'), cls=AuctionAction)
        self.players: Dict[int, AuctionPlayer] = utils.parse_players(json.get('players'), cls=AuctionPlayer)
        self.item = Item(json.get('item'))
        self.active_player_count = json.get('activePlayerCount')
        self.bid_count = json.get('bidCount')

        if self.bid_count > 0:
            self.highest_bid = AuctionAction(json.get('highestBid'))

    @property
    def any_bids(self) -> bool:
        return self.bid_count > 0

    @property
    def any_actions(self) -> bool:
        return len(self.actions) > 0
    
    @property
    def any_passed(self) -> bool:
        return self.active_player_count < len(self.players)

    @property
    def is_completed(self) -> bool:
        return self.active_player_count <= 1

    @property
    def current_amount(self) -> int:
        if self.any_bids: return self.highest_bid.bid_amount
        else: return self.item.price

class AuctionPlayer():
    def __init__(self, json: dict):
        self.name = json.get('name')
        self.player_id = json.get('playerId')
        self.discord_id = json.get('discordId')
        self.points = json.get('points')
        self.is_active = json.get('isActive')
        self.game_player_id = json.get('gamePlayerId')

        if self.game_player_id is not None:
            random.seed(self.game_player_id)
        else:
            random.seed(self.player_id)

        self.avatar = random.choice(utils.PLAYER_AVATARS)

class AuctionAction():
    def __init__(self, json: dict):
        self.player_id = json.get('playerId')
        self.is_pass = json.get('isPass')
        self.bid_amount = json.get('bidAmount')
        self.date_created = utils.parse_date(json.get('dateCreated'))

class AuctionSummary():
    def __init__(self, json: dict):
        self.final_price = json.get('finalPrice')
        self.item = Item(json.get('item'))
        self.has_winner = json.get('hasWinner')

        if self.has_winner:
            self.winner = AuctionPlayer(json.get('winner'))
