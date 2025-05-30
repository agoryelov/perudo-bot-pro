from .auction import AuctionSetup
from .player import Player

import utils

class GameSetup():
    def __init__(self, json: dict):
        self.players : dict[int, Player] = utils.parse_players(json.get('players'), Player)
        self.discord_players : dict[int, Player] = utils.parse_discord_players(json.get('players'))
        self.round_type : str = utils.parse_round_type(json.get('defaultRoundType'))
        self.game_id : int = json.get('gameId')
        self.show_auction = json.get('auctionSetup') is not None
        self.show_auction = False

        if self.show_auction:
            self.daily_auction : AuctionSetup = AuctionSetup(json.get('auctionSetup'))
    
    def auction_setup(self, starting_id):
        return { 'AuctionId': self.daily_auction.id, 'DiscordIds': list(self.discord_players.keys()), 'StartingDiscordId': starting_id }

    def can_start_game(self) -> bool:
        return len(self.discord_players) >= utils.MIN_GAME_PLAYERS
    
    def can_start_auction(self, starting_id = None) -> bool:
        if self.eligible_auction_players < utils.MIN_AUCTION_PLAYERS:
            return False

        if starting_id is None:
            return True
        
        starting_player = self.discord_players[starting_id]
        return starting_player.points >= self.daily_auction.item.price

    @property
    def eligible_auction_players(self) -> int:
        if not self.show_auction: return 0
        eligible_players = 0
        for player in self.discord_players.values():
            if player.points >= self.daily_auction.item.price and not player.is_bot:
                eligible_players += 1
        return eligible_players