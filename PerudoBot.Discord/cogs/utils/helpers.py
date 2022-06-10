from typing import Tuple


def get_emoji(num : int) -> str:
    if num == 1: return ':one:'
    if num == 2: return ':two:'
    if num == 3: return ':three:'
    if num == 4: return ':four:'
    if num == 5: return ':five:'
    if num == 6: return ':six:'
    else: return ':grey_question:'

def get_mention(discord_id: int) -> str:
    return f'<@!{discord_id}>'

def parse_bid(bid_text: str) -> Tuple[int, int]:
    bid_text = bid_text.strip().split()

    if len(bid_text) < 2:
        return None
    
    for num in bid_text:
        if not num.isnumeric():
            return False

    return int(bid_text[0]), int(bid_text[1])
