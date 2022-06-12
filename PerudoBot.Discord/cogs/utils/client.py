from discord import Message
import requests
from cogs.models.player import Player
from cogs.models.shared import parse_discord_players
from cogs.utils.constants import GameState

import urllib3
urllib3.disable_warnings()

SERVER_PATH = 'https://localhost:7068'

class ServerResponse:
    def __init__(self, response : dict = None) -> None:
        self.is_success = False
        self.data : dict = None
        self.error_message = 'Can\'t do that right now'

        if response is None: return
        
        status_code = response['statusCode']
        body : dict = response['value']
        
        self.is_success = status_code == 200
        self.data = body.get('data')
        self.error_message = body.get('error')

        if not self.is_success:
            print(response)
    
class GameClient():
    def __init__(self) -> None:
        self.players : dict[int, Player] = None
        self.game_id : int = None
        self.round_message : Message = None
        self.game_state = GameState.Terminated

        self.has_bots = False
        self.bot_message : Message = None
    
    @staticmethod
    def get_ladder_info() -> ServerResponse:
        return GameClient._get(f'{SERVER_PATH}/stats/ladder')
    
    def game_in_progress(self) -> bool:
        return self.game_state == GameState.InProgress

    def add_note(self, discord_id, note_text) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id), 'PLAYER_ID': str(self._player_id(discord_id)) }
        payload = { 'Text': note_text }
        return GameClient._post(f'{SERVER_PATH}/game/note', headers=headers, payload=payload)
    
    def set_default_round_type(self, round_type: str) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        return GameClient._post(f'{SERVER_PATH}/game/roundtype/{round_type}', headers=headers)

    def current_round(self) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        return GameClient._get(f'{SERVER_PATH}/game/round', headers=headers)

    def create_game(self) -> ServerResponse:
        response = self._post(f'{SERVER_PATH}/game/create')
        if response.is_success:
            self.game_id = response.data['gameId']
            self.game_state = GameState.Setup
        return response

    def end_game(self) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        response = self._post(f'{SERVER_PATH}/game/end', headers=headers)
        if response.is_success:
            self.game_state = GameState.Ended
        return response

    def add_player(self, discord_id, name, is_bot = False) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        payload = { 'DiscordId': discord_id, 'Name': name }
        response = GameClient._post(f'{SERVER_PATH}/game/addplayer', headers=headers, payload=payload)
        if response.is_success and is_bot:
            self.has_bots = is_bot
        return response
    
    def start_game(self) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        response = GameClient._post(f'{SERVER_PATH}/game/start', headers=headers)
        if response.is_success:
            self.game_state = GameState.InProgress
        return response
    
    def terminate_game(self) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        response = GameClient._post(f'{SERVER_PATH}/game/terminate', headers=headers)
        if response.is_success:
            self.game_state = GameState.Terminated
        return response

    def start_round(self) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id) }
        response = GameClient._post(f'{SERVER_PATH}/game/newround', headers=headers)
        if response.is_success:
            self.players = parse_discord_players(response.data['players'])
        return response

    def bet_action(self, discord_id, amount : int, bet_type: str) -> ServerResponse:
        bet_type_id = 0 if bet_type == 'exact' else 1
        headers = { 'GAME_ID': str(self.game_id), 'PLAYER_ID': str(self._player_id(discord_id)) }
        payload = { 'Amount': amount, 'Type': bet_type_id }
        response = GameClient._post(f'{SERVER_PATH}/game/bet', headers=headers, payload=payload)
        if response.is_success:
            self.players = parse_discord_players(response.data['players'])
        return response

    def liar_action(self, discord_id) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id), 'PLAYER_ID': str(self._player_id(discord_id)) }
        response = GameClient._post(f'{SERVER_PATH}/game/liar', headers=headers)
        return response

    def bid_action(self, discord_id, quantity, pips) -> ServerResponse:
        headers = { 'GAME_ID': str(self.game_id), 'PLAYER_ID': str(self._player_id(discord_id)) }
        payload = { 'Quantity': quantity, 'Pips': pips }
        response = GameClient._post(f'{SERVER_PATH}/game/bid', headers=headers, payload=payload)
        return response

    def get_player(self, discord_id):
        return self.players[discord_id]
    
    @staticmethod
    def _get(url, headers = None) -> ServerResponse:
        r = requests.get(url, headers=headers, verify=False)
        return ServerResponse(r.json())

    @staticmethod
    def _post(url, headers = None, payload = None) -> ServerResponse:
        r = requests.post(url, headers=headers, json=payload, verify=False)
        return ServerResponse(r.json())
    
    def _player_id(self, discord_id):
        return self.players[discord_id].player_id