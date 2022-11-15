import enum

class GameState(enum.Enum):
    Setup = 0
    InProgress = 1
    Terminated = 2
    Ended = 3

class BetType(enum.IntEnum):
    Exact = 0
    Liar = 1
    Peak = 2
    Legit = 3

class RattleType(enum.IntEnum):
    Death = 0
    Win = 1
    Taunt = 2

class RattleContentType(enum.IntEnum):
    Image = 0
    Sound = 1

class EmbedColor(enum.IntEnum):
    Red = 12199168
    White = 16777215
    Yellow = 16172079
    Invisible = 3092790

class MessageType(enum.IntEnum):
    Setup = 0
    Auction = 1
    Round = 2
    Bets = 3

SYM_X = "×"
EMPTY = "‎"

MIN_GAME_PLAYERS = 2
MIN_AUCTION_PLAYERS = 3

PLAYER_AVATARS = [':apple:',':tangerine:',':watermelon:',':grapes:',':green_apple:',':strawberry:',':kiwi:',':blueberries:']