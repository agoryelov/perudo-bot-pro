DEFAULT = ['<:default_01:1033119254154846210>','<:default_02:1033119254951755948>','<:default_03:1033119256310710292>','<:default_04:1033119257040539751>','<:default_05:1033119258575642754>','<:default_06:1033119259657764976>']
SYSTEM = ['<:default_01:1033119254154846210>','<:default_02:1033119254951755948>','<:default_03:1033119256310710292>','<:default_04:1033119257040539751>','<:default_05:1033119258575642754>','<:default_06:1033119259657764976>']

def system_emote(num):
    return DEFAULT[num - 1]

def player_emote(num, dice: str = None):
    if dice is None:
        return DEFAULT[num - 1]
    
    custom_emotes = dice.split(',')
    return custom_emotes[num - 1]

def _format_dice_preview(emote_list):
    return ' '.join(emote_list)

def dice_preview(emote_str : str = None):
    if emote_str is None:
        return _format_dice_preview(DEFAULT)
    emote_list = emote_str.split(',')
    return _format_dice_preview(emote_list)