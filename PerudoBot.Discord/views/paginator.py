import discord
from abc import ABC, abstractmethod

class PageSource(ABC):
    @property
    @abstractmethod
    def num_pages(self) -> int:
        pass

    @abstractmethod
    def page(self, page_num: int) -> discord.Embed:
        pass

class PagedView(discord.ui.View):
    def __init__(self, source: PageSource):
        super().__init__(timeout=1200)
        self.source = source
        self.current_page = 1
        self.num_pages = source.num_pages
        self.update_controls()

    def update_controls(self):
        self.clear_items()
        self.add_item(FirstPage(self))
        self.add_item(PrevPage(self))
        self.add_item(CurrentPage(self))
        self.add_item(NextPage(self))
        self.add_item(LastPage(self))
    
    async def on_timeout(self) -> None:
        for item in self.children:
            item.disabled = True

        await self.message.edit(view=self)

class FirstPage(discord.ui.Button):
    def __init__(self, view: PagedView):
        super().__init__()
        self.paged_view = view
        self.style = discord.ButtonStyle.grey
        self.label = '<<'
        self.disabled = view.current_page == 1

    async def callback(self, interaction: discord.Interaction):
        pager = self.paged_view
        pager.current_page = 1
        pager.update_controls()
        await interaction.response.edit_message(view=pager, embed=pager.source.page(pager.current_page))

class LastPage(discord.ui.Button):
    def __init__(self, view: PagedView):
        super().__init__()
        self.paged_view = view
        self.style = discord.ButtonStyle.grey
        self.label = '>>'
        self.disabled = view.current_page == view.num_pages

    async def callback(self, interaction: discord.Interaction):
        pager = self.paged_view
        pager.current_page = pager.num_pages
        pager.update_controls()
        await interaction.response.edit_message(view=pager, embed=pager.source.page(pager.current_page))

class PrevPage(discord.ui.Button):
    def __init__(self, view: PagedView):
        super().__init__()
        self.paged_view = view
        self.style = discord.ButtonStyle.blurple
        
        prev_page = view.current_page - 1
        self.label = '\u200b' if prev_page < 1 else str(prev_page)
        self.disabled = prev_page < 1

    async def callback(self, interaction: discord.Interaction):
        pager = self.paged_view
        pager.current_page -= 1
        pager.update_controls()
        await interaction.response.edit_message(view=pager, embed=pager.source.page(pager.current_page))

class CurrentPage(discord.ui.Button):
    def __init__(self, view: PagedView):
        super().__init__()
        self.style = discord.ButtonStyle.gray
        self.disabled = True
        self.label = str(view.current_page)

class NextPage(discord.ui.Button):
    def __init__(self, view: PagedView):
        super().__init__()
        self.paged_view = view
        self.style = discord.ButtonStyle.blurple

        next_page = view.current_page + 1
        self.label = '\u200b' if next_page > view.num_pages else str(next_page)
        self.disabled = next_page > view.num_pages
    
    async def callback(self, interaction: discord.Interaction):
        pager = self.paged_view
        pager.current_page += 1
        pager.update_controls()
        await interaction.response.edit_message(view=pager, embed=pager.source.page(pager.current_page))