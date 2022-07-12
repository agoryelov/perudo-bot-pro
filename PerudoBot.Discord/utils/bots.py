from base64 import b64encode
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad
from Crypto.Protocol.KDF import PBKDF2
from Crypto.Random import get_random_bytes

import json
from models import Round
from models.player import Player

def bot_update(round: Round, channel_id: int, points: int):
    message = {
        'points': points,
        'channel': str(channel_id),
        'round': round.round_number, 
        'gameDice': round.total_dice,
        'nextPlayer' : round.players[round.action_player_id].discord_id
    }

    if round.latest_bid is None:
        return json.dumps(message)

    current_player = round.players[round.latest_bid.player_id]
    message['currentPlayer'] = current_player.discord_id
    message['playerDice'] = len(current_player.dice)
    message['action'] = bid_to_action_index(round.latest_bid.quantity, round.latest_bid.pips, round.total_dice)
    return json.dumps(message)

def bot_dice(player: Player, channel_id: int):
    message = {
        'channel': str(channel_id),
        'dice': encrypt_dice(player.name, player.dice)
    }
    return json.dumps(message)

def encrypt_dice(key : str, dice: list[int]) -> str:
    data = ' '.join(str(x) for x in dice)
    data = bytes(data, encoding='utf-8')
    salt = get_random_bytes(16)
    salted_key = PBKDF2(key, salt, 32, count=10000)

    cipher = AES.new(salted_key, AES.MODE_CBC)
    ct_bytes = cipher.encrypt(pad(data, AES.block_size))

    result = bytearray(salt)
    result.extend(cipher.iv)
    result.extend(ct_bytes)
    
    return b64encode(result).decode('utf-8')

def bid_to_action_index(quantity: int, pips: int, total_dice: int) -> int:
    if pips != 1:
        wildcard = quantity // 2
        non_wildcard = (quantity - 1) * 5
        return wildcard + non_wildcard + (pips - 2)
    else:
        absolute_index = 5 + ((quantity - 1) * 11)
        adjust = max(quantity - 1 - (total_dice - quantity), 0) * 5
        return absolute_index - adjust