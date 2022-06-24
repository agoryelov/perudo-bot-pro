from typing import Tuple
from datetime import datetime
from models import Round, Player

def get_emoji(num : int) -> str:
    if num == 1: return ':one:'
    if num == 2: return ':two:'
    if num == 3: return ':three:'
    if num == 4: return ':four:'
    if num == 5: return ':five:'
    if num == 6: return ':six:'
    else: return ':grey_question:'

def get_unicode(num : int) -> str:
    if num == 1: return '1️⃣'
    if num == 2: return '2️⃣'
    if num == 3: return '3️⃣'
    if num == 4: return '4️⃣'
    if num == 5: return '5️⃣'
    if num == 6: return '6️⃣'
    else: return '1️⃣'

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

def bid_to_action_index(quantity: int, pips: int) -> int:
    if pips != 1:
        wildcard = quantity // 2
        non_wildcard = (quantity - 1) * 5
        return wildcard + non_wildcard + (pips - 2)
    else:
        return 5 + ((quantity - 1) * 11)

def next_up_message(round: Round) -> str:
    next_player = f'<@!{round.players[round.action_player_id].discord_id}>'
    # bot_update = f' ||`@bots update {game_driver.bot_message.id}`||' if game_driver.has_bots else ''
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
    return f'`Your dice`: {" ".join(get_emoji(x) for x in player.dice)}'