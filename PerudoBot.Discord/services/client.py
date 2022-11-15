import requests
from utils import GameActionError

import urllib3
urllib3.disable_warnings()

from dotenv import load_dotenv
from os import getenv

load_dotenv()

SERVER_PATH = getenv('SERVER_PATH', 'https://localhost:7068')

class Client():
    def get_achievements():
        return Client._get(f'{SERVER_PATH}/general/achievements')

    def get_user_achievements(discord_id):
        return Client._get(f'{SERVER_PATH}/general/achievements/{discord_id}')

    def get_ladder_info():
        return Client._get(f'{SERVER_PATH}/general/ladder')

    def get_user_profile(discord_id):
        return Client._get(f'{SERVER_PATH}/general/profile/{discord_id}')

    def get_user_inventory(discord_id):
        return Client._get(f'{SERVER_PATH}/general/inventory/{discord_id}')

    def equip_dice_item(discord_id, item_id = -1):
        payload = { 'DiscordId': discord_id, 'ItemId': item_id }
        return Client._post(f'{SERVER_PATH}/general/inventory/equip', payload=payload)

    def update_rattle(discord_id, content, type, content_type = 0):
        payload = { 'DiscordId': discord_id, 'Content': content, 'RattleType': type, 'RattleContentType': content_type }
        return Client._post(f'{SERVER_PATH}/general/rattles', payload=payload)

    def add_note(game_id, player_id, note_text):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        payload = { 'Text': note_text }
        return Client._post(f'{SERVER_PATH}/game/note', headers=headers, payload=payload)
    
    def set_default_round_type(game_id, round_type):
        headers = { 'GAME_ID': str(game_id) }
        return Client._post(f'{SERVER_PATH}/game/roundtype/{round_type}', headers=headers)

    def resume_game(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._get(f'{SERVER_PATH}/game/resume', headers=headers)

    def current_round(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._get(f'{SERVER_PATH}/game/round', headers=headers)

    def round_summary(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._get(f'{SERVER_PATH}/game/roundsummary', headers=headers)

    def fetch_setup(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._get(f'{SERVER_PATH}/game/setup', headers=headers)

    def create_game():
        return Client._post(f'{SERVER_PATH}/game/create')

    def end_game(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._post(f'{SERVER_PATH}/game/end', headers=headers)

    def add_player(game_id, discord_id, name, is_bot = False):
        headers = { 'GAME_ID': str(game_id) }
        payload = { 'DiscordId': discord_id, 'Name': name, 'IsBot': is_bot }
        return Client._post(f'{SERVER_PATH}/game/addplayer', headers=headers, payload=payload)
    
    def start_game(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._post(f'{SERVER_PATH}/game/start', headers=headers)
    
    def terminate_game(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._post(f'{SERVER_PATH}/game/terminate', headers=headers)

    def start_round(game_id):
        headers = { 'GAME_ID': str(game_id) }
        return Client._post(f'{SERVER_PATH}/game/newround', headers=headers)

    def bet_action(game_id, player_id, amount : int, bet_type: int, target_id: int):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        payload = { 'Amount': amount, 'Type': bet_type, 'TargetBidId': target_id }
        return Client._post(f'{SERVER_PATH}/game/bet', headers=headers, payload=payload)

    def reverse_action(game_id, player_id):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        return Client._post(f'{SERVER_PATH}/game/reverse', headers=headers)

    def liar_action(game_id, player_id):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        return Client._post(f'{SERVER_PATH}/game/liar', headers=headers)
    
    def bid_action(game_id, player_id, quantity, pips):
        headers = { 'GAME_ID': str(game_id), 'PLAYER_ID': str(player_id) }
        payload = { 'Quantity': quantity, 'Pips': pips }
        return Client._post(f'{SERVER_PATH}/game/bid', headers=headers, payload=payload)

    def start_auction(game_setup):
        return Client._post(f'{SERVER_PATH}/auction/start', payload=game_setup)

    def auction_bid(auction_id, discord_id, amount):
        headers = { 'AUCTION_ID': str(auction_id) }
        payload = { 'DiscordId': discord_id, 'Amount': amount }
        return Client._post(f'{SERVER_PATH}/auction/bid', headers=headers, payload=payload)

    def auction_pass(auction_id, discord_id):
        headers = { 'AUCTION_ID': str(auction_id) }
        payload = { 'DiscordId': discord_id }
        return Client._post(f'{SERVER_PATH}/auction/pass', headers=headers, payload=payload)

    def end_auction(auction_id):
        headers = { 'AUCTION_ID': str(auction_id) }
        return Client._post(f'{SERVER_PATH}/auction/end', headers=headers)

    @staticmethod
    def _get(url, headers = None):
        r = requests.get(url, headers=headers, verify=False)
        return Client._get_data(r.json())

    @staticmethod
    def _post(url, headers = None, payload = None):
        r = requests.post(url, headers=headers, json=payload, verify=False)
        return Client._get_data(r.json())

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

