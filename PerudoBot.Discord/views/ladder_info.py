import discord
from prettytable import PrettyTable
from models import LadderEntry

class LadderInfoView(discord.ui.View):
    def __init__(self, ladder_entries: list[LadderEntry]):
        super().__init__()
        self.entries = ladder_entries
    
    @discord.ui.select(placeholder="Choose ladder...", options=[
        discord.SelectOption(label='Points', value='Points'),
        discord.SelectOption(label='Elo Rating', value='Elo'),
        discord.SelectOption(label='Achievement Score', value='Score')
    ])
    async def choose(self, interaction: discord.Interaction, select: discord.ui.Select):
        await interaction.response.edit_message(view=self, embed=LadderInfoEmbed(self.entries, ladder_type=select.values[0]))
    
    async def on_timeout(self) -> None:
        for item in self.children:
            item.disabled = True

        await self.message.edit(view=self)

class LadderInfoEmbed(discord.Embed):
    def __init__(self, ladder_entries: list[LadderEntry], ladder_type):
        super().__init__(title=f'Ladder')
        self.entries = ladder_entries
        self.sort_entries(ladder_type)
        self.add_field(name='Standings', value=self.get_standings(ladder_type))

    def get_standings(self, ladder_type):
        table = PrettyTable()
        table.field_names = ["Name", ladder_type]
        for entry in self.entries:
            if ladder_type == 'Points': table.add_row([entry.name, entry.points])
            if ladder_type == 'Elo': table.add_row([entry.name, entry.elo])
            if ladder_type == 'Score': table.add_row([entry.name, entry.achievement_score])
        
        table.align["Name"] = "l"
        table.align[ladder_type] = "l"
        return f'```\n{table.get_string(border=False)}\n```'
    
    def sort_entries(self, ladder_type):
        if ladder_type == 'Points':
            self.entries = sorted(self.entries, key=lambda x: x.points, reverse=True)
        if ladder_type == 'Elo':
            self.entries = sorted(self.entries, key=lambda x: x.elo, reverse=True)
        if ladder_type == 'Score':
            self.entries = sorted(self.entries, key=lambda x: x.achievement_score, reverse=True)