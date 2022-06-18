import asyncio
from typing import Union

import discord
from discord import Member, Message, User

from models import Player, Round, GameSetup, RoundSummary, GameSummary
from utils import GameState, get_emoji, next_up_message, get_mention
from utils.exceptions import GameActionError
from views import RoundEmbed
from .client import GameClient

class GameDriver():
    def __init__(self, channel: discord.TextChannel):
        self.channel = channel
        self.game_client = GameClient()
        
        self.game_state = GameState.Terminated
        self.game_id = 0
        self.discord_players : dict[int, Player] = {}

        self.round_message : Message = None
        self.next_up_message : Message = None
        self.bot_message : Message = None

    async def create_game(self) -> GameSetup:
        setup_data = self.game_client.create_game()
        game_setup = GameSetup(setup_data)
        await self._update_from_setup(game_setup)
        return game_setup

    async def set_default_round_type(self, round_type) -> GameSetup:
        setup_data = self.game_client.set_default_round_type(self.game_id, round_type)
        game_setup = GameSetup(setup_data)
        await self._update_from_setup(game_setup)
        return game_setup
    
    async def add_player(self, user: Union[User, Member]) -> GameSetup:
        name = user.nick or user.name
        setup_data = self.game_client.add_player(self.game_id, user.id, name, user.bot)
        game_setup = GameSetup(setup_data)
        await self._update_from_setup(game_setup)
        return game_setup

    async def start_game(self):
        self.game_client.start_game(self.game_id)
        self.game_state = GameState.InProgress
        if self.has_bots: await self._send_bot_message()

    async def start_round(self) -> Round:
        round_data = self.game_client.start_round(self.game_id)
        round = Round(round_data)
        self.round_message = await self.channel.send(embed=RoundEmbed(round))
        self.next_up_message = await self.channel.send(next_up_message(round, self))
        await self._update_from_round(round)
        return round

    async def bid_action(self, discord_id, quantity, pips) -> Round:
        round_data = self.game_client.bid_action(self.game_id, self._player_id(discord_id), quantity, pips)
        round = Round(round_data)
        await self._update_from_round(round)
        return round
    
    async def bet_action(self, discord_id, amount, bet_type) -> Round:
        round_data = self.game_client.bet_action(self.game_id, self._player_id(discord_id), amount, bet_type)
        round = Round(round_data)
        await self._update_from_round(round)
        return round

    async def liar_action(self, discord_id) -> RoundSummary:
        summary_data = self.game_client.liar_action(self.game_id, self._player_id(discord_id))
        round_summary = RoundSummary(summary_data)
        await self.next_up_message.delete()
        await self._update_from_round(round_summary.round)
        return round_summary


    async def end_game(self) -> GameSummary:
        summary_data = self.game_client.end_game(self.game_id)
        return GameSummary(summary_data)
    
    async def terminate(self):
        await self.game_client.terminate_game(self.game_id)
    
    async def _update_from_setup(self, game_setup: GameSetup):
        self.game_id = game_setup.game_id
        self.discord_players = game_setup.discord_players
        self.game_state = GameState.Setup

    async def update_round_message(self, round: Round):
        recent_history = [message async for message in self.channel.history(limit=2)]
        if self.round_message in recent_history:
            await self.round_message.edit(embed=RoundEmbed(round))
            await self.next_up_message.delete()
            self.next_up_message = await self.channel.send(next_up_message(round, self))
        else:
            await self.round_message.delete()
            await self.next_up_message.delete()
            self.round_message = await self.channel.send(embed=RoundEmbed(round))
            self.next_up_message = await self.channel.send(next_up_message(round, self))

    async def send_liar_result(self, round_summary: RoundSummary):
        round = round_summary.round
        liar_action = round.liar
        bid_player = round.players[liar_action.target_bid.player_id]
        liar_player = round.players[liar_action.player_id]
        losing_player = round.players[liar_action.losing_player_id]

        await self.channel.send(f':fire: **{liar_player.name}** called liar on **{bid_player.name}**')
        await asyncio.sleep(2)
        await self.channel.send(f':fire: There was actually `{round.liar.actual_quantity}` ˣ {get_emoji(round.latest_bid.pips)}. {get_mention(losing_player.discord_id)} loses `{liar_action.lives_lost}` ˣ :heart:')
        await asyncio.sleep(1)

        if losing_player.lives <= 0:
            winning_player = liar_player if liar_action.is_successful else bid_player
            await self.channel.send(f':skull: **{losing_player.name}** was defeated by **{winning_player.name}**')
    
    async def _update_from_round(self, round: Round):
        self.discord_players = round.discord_players
        self.game_state = GameState.InProgress if not round.is_final else GameState.Ended
        if self.has_bots: await self._update_bot_message(round)

    async def _send_bot_message(self):
        self.bot_message = await self.channel.send('||`{}`||')

    async def _update_bot_message(self, round: Round):
        await self.bot_message.edit(content=f'||`{round.bot_message()}`||')
    
    def _player_id(self, discord_id):
        if discord_id not in self.discord_players:
            raise GameActionError("Player not in game")
        return self.discord_players[discord_id].player_id
    
    @property
    def terminated(self) -> bool:
        return self.game_state == GameState.Terminated

    @property
    def in_setup(self) -> bool:
        return self.game_state == GameState.Setup

    @property
    def in_progress(self) -> bool:
        return self.game_state == GameState.InProgress
    
    @property
    def ended(self) -> bool:
        return self.game_state == GameState.Ended
    
    @property
    def has_bots(self) -> bool:
        if self.discord_players is None: return False
        for player in self.discord_players.values():
            if player.lives > 0 and player.is_bot:
                return True
        return False

        


