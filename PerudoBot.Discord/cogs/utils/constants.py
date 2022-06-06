import enum

class GameState(enum.Enum):
    Setup = 0
    InProgress = 1
    Terminated = 2
    Ended = 3

class BetType(enum.Enum):
    Exact = 0
    Liar = 1