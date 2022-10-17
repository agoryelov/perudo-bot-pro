import requests

from typing import Tuple
from datetime import datetime
from models import Round, Player
from .constants import BetType
from .items import dice_emote

def get_mention(discord_id: int) -> str:
    return f'<@!{discord_id}>'

def parse_date(date_string) -> datetime:
    return datetime.strptime(date_string, '%Y-%m-%dT%H:%M:%S%z')

def get_next_bid(quantity, pips) -> Tuple[int, int]:
    if pips > 1 and pips < 6:
        return quantity, (pips + 1)

    if pips == 1:
        return quantity * 2, 2
    
    if pips == 6 and (quantity % 2 != 0):
        return (quantity // 2 + 1), 1
    
    return quantity + 1, 2

def get_pip_quantity(pips, prev_quantity, prev_pips):
    if pips == 1 and prev_pips == 1:
        return prev_quantity + 1
    
    if prev_pips == 1:
        return prev_quantity * 2
    
    if pips == 1:
        return (prev_quantity // 2 + 1)
    
    if pips <= prev_pips:
        return prev_quantity + 1

    return prev_quantity

def next_up_message(round: Round) -> str:
    next_player = f'<@!{round.players[round.action_player_id].discord_id}>'
    return f'{next_player} is up next'

def parse_bid(bid_text: str) -> Tuple[int, int]:
    bid_text = bid_text.strip().split()

    if len(bid_text) < 2:
        return None
    
    for num in bid_text:
        if not num.isnumeric():
            return False

    return int(bid_text[0]), int(bid_text[1])

def deal_dice_message(player: Player):
    return f'`Your dice`: {" ".join(dice_emote(x, player.equipped_dice) for x in player.dice)}'

def bet_emoji(type: BetType) -> str:
    if type is BetType.Liar: return '🧊'
    if type is BetType.Legit: return '🔥'
    if type is BetType.Exact: return '🎯'
    if type is BetType.Peak: return '🗻'
    return '❔'

def min_bet(type: BetType) -> int:
    if type is BetType.Liar: return 50
    if type is BetType.Legit: return 50
    if type is BetType.Exact: return 50
    if type is BetType.Peak: return 50
    return 0

def is_url_image(image_url) -> bool:
    image_formats = ("image/png", "image/jpeg", "image/jpg", "image/gif")

    try: 
        r = requests.head(image_url)
        return r.status_code == 200 and r.headers["content-type"] in image_formats
    except: 
        return False