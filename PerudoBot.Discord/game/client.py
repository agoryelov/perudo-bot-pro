import requests
from utils import GameActionError

import urllib3
urllib3.disable_warnings()

SERVER_PATH = 'https://localhost:7068'

class GameClient():

    @staticmethod
    def get_achievements():
        return GameClient._get(f'{SERVER_PATH}/general/achievements')

    @staticmethod
    def get_user_achievements(discord_id):
        return GameClient._get(f'{SERVER_PATH}/general/achievements/{discord_id}')

    @staticmethod
    def get_ladder_info():
        return GameClient._get(f'{SERVER_PATH}/general/ladder')

    def add_note(self, game_id, player_id, note_text):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        payload = { 'Text': note_text }
        return GameClient._post(f'{SERVER_PATH}/game/note', headers=headers, payload=payload)
    
    def set_default_round_type(self, game_id, round_type):
        headers = { 'GAME_ID': str(game_id) }
        return GameClient._post(f'{SERVER_PATH}/game/roundtype/{round_type}', headers=headers)

    def current_round(self, game_id):
        headers = { 'GAME_ID': str(game_id) }
        return GameClient._get(f'{SERVER_PATH}/game/round', headers=headers)

    def create_game(self):
        return GameClient._post(f'{SERVER_PATH}/game/create')

    def end_game(self, game_id):
        headers = { 'GAME_ID': str(game_id) }
        return GameClient._post(f'{SERVER_PATH}/game/end', headers=headers)

    def add_player(self, game_id, discord_id, name, is_bot = False):
        headers = { 'GAME_ID': str(game_id) }
        payload = { 'DiscordId': discord_id, 'Name': name, 'IsBot': is_bot }
        return GameClient._post(f'{SERVER_PATH}/game/addplayer', headers=headers, payload=payload)
    
    def start_game(self, game_id):
        headers = { 'GAME_ID': str(game_id) }
        return GameClient._post(f'{SERVER_PATH}/game/start', headers=headers)
    
    def terminate_game(self, game_id):
        headers = { 'GAME_ID': str(game_id) }
        return GameClient._post(f'{SERVER_PATH}/game/terminate', headers=headers)

    def start_round(self, game_id):
        headers = { 'GAME_ID': str(game_id) }
        return GameClient._post(f'{SERVER_PATH}/game/newround', headers=headers)

    def bet_action(self, game_id, player_id, amount : int, bet_type: str):
        bet_type_id = 0 if bet_type == 'exact' else 1
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        payload = { 'Amount': amount, 'Type': bet_type_id }
        return GameClient._post(f'{SERVER_PATH}/game/bet', headers=headers, payload=payload)

    def liar_action(self, game_id, player_id):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        return GameClient._post(f'{SERVER_PATH}/game/liar', headers=headers)
    
    def bid_action(self, game_id, player_id, quantity, pips):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        payload = { 'Quantity': quantity, 'Pips': pips }
        return GameClient._post(f'{SERVER_PATH}/game/bid', headers=headers, payload=payload)

    @staticmethod
    def _get(url, headers = None):
        r = requests.get(url, headers=headers, verify=False)
        return GameClient._get_data(r.json())

    @staticmethod
    def _post(url, headers = None, payload = None):
        r = requests.post(url, headers=headers, json=payload, verify=False)     
        return GameClient._get_data(r.json())

    @staticmethod
    def _get_data(response: dict = None):
        if response is None: 
            raise GameActionError()

        body : dict = response.get('value')
        if body is None: 
            raise GameActionError()

        status_code = response.get('statusCode')
        if status_code is None or status_code != 200:
             raise GameActionError(body.get('error'))

        return body.get('data')

