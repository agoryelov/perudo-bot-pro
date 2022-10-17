from models import Player

DEFAULT = ['<:default_01:1030972676891758662>', '<:default_02:1030972677550252156>', '<:default_03:1030972678997278832>', '<:default_04:1030972680154910740>', '<:default_05:1030972681249628171>', '<:default_06:1030972682319183944>']

def dice_emote(num, dice : str = None):
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