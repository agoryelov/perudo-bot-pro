import discord
from discord import File
from prettytable import PrettyTable
from easy_pil import Editor, Canvas

from models import LadderInfo

class LadderInfoEmbed(discord.Embed):
    def __init__(self, ladder_info: LadderInfo):
        super().__init__(title=f'Ladder')
        self.ladder_info = ladder_info
        self.add_field(name='Standings', value=self.get_standings())

        board = Canvas(width=500, height=500)
        editor = Editor(board)
        editor.text((10, 10), "Hello World")
        self.test_filename = 'test_image.png'
        self.test_file = File(fp=editor.image_bytes, filename=self.test_filename)
    
    def get_standings(self):
        x = PrettyTable()
        x.field_names = ["Name", "Elo", "Points"]
        for entry in self.ladder_info.entries:
            x.add_row([entry.name, entry.elo, entry.points])
        
        x.align["Name"] = "l"
        x.align["Elo"] = "l"
        x.align["Points"] = "l"
        return f'```\n{x.get_string(border=False)}\n```'