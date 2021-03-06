import enum

class GameState(enum.Enum):
    Setup = 0
    InProgress = 1
    Terminated = 2
    Ended = 3

class BetType(enum.IntEnum):
    Exact = 0
    Liar = 1,
    Peak = 2,
    Legit = 3

class EmbedColor(enum.IntEnum):
    Red = 12199168
    White = 16777215
    Yellow = 16172079
    Invisible = 3092790

SYM_X = "×"