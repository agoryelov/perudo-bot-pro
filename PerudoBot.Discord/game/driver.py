import asyncio
from typing import Union


import discord
from discord import Member, Message, TextChannel, User, VoiceClient

from models import Player, Round, GameSetup, RoundSummary, GameSummary
from utils import GameState, get_emoji, GameActionError, bot_dice, bot_update, get_mention
from views import RoundEmbed, RoundView, LiarCalledEmbed, DamageDealtEmbed, DefeatEmbed
from .client import GameClient

class GameDriver():

    def __init__(self, game_channel: discord.TextChannel):
        self.channel = game_channel
        self.game_client = GameClient()
        
        self.game_state = GameState.Terminated
        self.game_id = 0
        self.discord_players : dict[int, Player] = {}

        self.round_message : Message = None
        self.voice_client : VoiceClient = None
        self.bot_channel : TextChannel = None

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

    async def start_round(self) -> Round:
        round_data = self.game_client.start_round(self.game_id)
        round = Round(round_data)
        self.round_message = await self.send_delayed(view=RoundView(round, self), embed=RoundEmbed(round))
        await self._update_from_round(round)
        await self._send_out_dice(round)
        if self.has_bots: await self._send_bot_updates(round)
        return round

    async def bid_action(self, discord_id, quantity, pips) -> Round:
        round_data = self.game_client.bid_action(self.game_id, self._player_id(discord_id), quantity, pips)
        round = Round(round_data)
        await self._update_from_round(round)
        self._play_notification('notify_pop.mp3')
        if self.has_bots: await self._send_bot_updates(round)
        return round
    
    async def bet_action(self, discord_id, amount, bet_type) -> Round:
        round_data = self.game_client.bet_action(self.game_id, self._player_id(discord_id), amount, bet_type)
        round = Round(round_data)
        await self._update_from_round(round)
        self._play_notification('notify_coins.mp3')
        return round

    async def liar_action(self, discord_id) -> RoundSummary:
        summary_data = self.game_client.liar_action(self.game_id, self._player_id(discord_id))
        round_summary = RoundSummary(summary_data)
        await self._update_from_round(round_summary.round)
        return round_summary

    async def end_game(self) -> GameSummary:
        summary_data = self.game_client.end_game(self.game_id)
        return GameSummary(summary_data)
    
    async def terminate(self):
        self.game_client.terminate_game(self.game_id)
        self.game_state = GameState.Terminated
    
    async def _update_from_setup(self, game_setup: GameSetup):
        self.game_id = game_setup.game_id
        self.discord_players = game_setup.discord_players
        self.game_state = GameState.Setup

    def _play_notification(self, source = 'notify.mp3'):
        if self.voice_client is None: return
        try: self.voice_client.play(discord.FFmpegPCMAudio(executable='C:/audio/ffmpeg/bin/ffmpeg.exe', source=f'C:/audio/{source}'))
        except: pass

    async def update_round_message(self, round: Round, edit_function = None):
        edit_function = edit_function or self.round_message.edit
        recent_history = [message async for message in self.channel.history(limit=2)]
        if self.round_message in recent_history:
            await edit_function(view=RoundView(round, self), embed=RoundEmbed(round))
        else:
            await self.round_message.delete()
            self.round_message = await self.send_delayed(view=RoundView(round, self), embed=RoundEmbed(round), delay = 0)

    async def send_delayed(self, delay = 0.5, **kwargs):
        await asyncio.sleep(delay)
        return await self.channel.send(**kwargs)

    async def send_liar_result(self, round_summary: RoundSummary): 
        liar = round_summary.round.liar
        players = round_summary.round.players
        losing_player = players[liar.losing_player_id]

        liar_call_embed = await self.send_delayed(embed=LiarCalledEmbed(liar, players), delay=0)
        await asyncio.sleep(2)
        await liar_call_embed.edit(embed=LiarCalledEmbed(liar, players, show_actual=True))
        await self.send_delayed(embed=DamageDealtEmbed(liar, players), delay=1)
        
        if losing_player.lives <= 0:
            await self.send_delayed(embed=DefeatEmbed(liar, players), delay=1)
    
    async def _send_out_dice(self, round: Round):
        for player in round.players.values():
            if len(player.dice) <= 0: continue
            member = self.channel.guild.get_member(player.discord_id)
            if player.is_bot:
                await self._send_bot_dice(player)
            else:
                await member.send(f'`Your dice`: {" ".join(get_emoji(x) for x in player.dice)}')
    
    async def _update_from_round(self, round: Round):
        self.discord_players = round.discord_players
        self.game_state = GameState.InProgress if not round.is_final else GameState.Ended

    async def _send_bot_updates(self, round):
        if self.bot_channel is None:
            print("Warning: Bot channel is not accessible")
            return
        
        for discord_id, player in self.discord_players.items():
            if not player.is_bot: continue
            update = bot_update(round, self.channel.id, player.points)
            await self.bot_channel.send(f'{get_mention(discord_id)} update {update}')
    
    async def _send_bot_dice(self, player: Player):
        if self.bot_channel is None:
            print("Warning: Bot channel is not accessible")
            return
        
        await self.bot_channel.send(f'{get_mention(player.discord_id)} deal {bot_dice(player, self.channel.id)}')
    
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

        


