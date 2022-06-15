import discord
from prettytable import PrettyTable

from models import LadderEntry

class LadderInfoView(discord.ui.View):
    def __init__(self, ladder_entries: list[LadderEntry]):
        super().__init__()
        self.entries = ladder_entries
    
    @discord.ui.select(placeholder="Sort by...", options=[
        discord.SelectOption(label='Points', value='Points'),
        discord.SelectOption(label='Elo Rating', value='Elo Rating')
    ])
    async def sort(self, interaction: discord.Interaction, select: discord.ui.Select):

        if select.values[0] == 'Points':
            sorted_list: list[LadderEntry] = sorted(self.entries, key=lambda x: x.points, reverse=True)
        else:
            sorted_list: list[LadderEntry] = sorted(self.entries, key=lambda x: x.elo, reverse=True)
        
        await interaction.response.edit_message(view=self, embed=LadderInfoEmbed(sorted_list))
    
    async def on_timeout(self) -> None:
        for item in self.children:
            item.disabled = True

        await self.message.edit(view=self)

class LadderInfoEmbed(discord.Embed):
    def __init__(self, ladder_entries: list[LadderEntry]):
        super().__init__(title=f'Ladder')
        self.entries = ladder_entries
        self.add_field(name='Standings', value=self.get_standings())

    def get_standings(self):
        table = PrettyTable()
        table.field_names = ["Name", "Elo", "Points"]
        for entry in self.entries:
            table.add_row([entry.name, entry.elo, entry.points])
        
        table.align["Name"] = "l"
        table.align["Elo"] = "l"
        table.align["Points"] = "l"
        return f'```\n{table.get_string(border=False)}\n```'