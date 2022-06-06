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