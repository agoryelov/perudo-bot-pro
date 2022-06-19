class GameActionError(Exception):
    def __init__(self, message = 'Can\'t do that right now'):
        if message is None: self.message = 'Can\'t do that right now'
        else: self.message = message
    def __str__(self):
        return repr(self.message)