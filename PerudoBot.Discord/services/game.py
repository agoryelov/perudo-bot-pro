import asyncio
import discord

from os import getenv
from typing import Union, Dict, TYPE_CHECKING
from discord import Member, User, VoiceClient, VoiceChannel

from models import Player, Round, GameSetup, RoundSummary, GameSummary
from utils import MessageType, GameState, GameActionError, bot_dice, bot_update, get_mention
from views import LiarCalledEmbed, DamageDealtEmbed, DefeatEmbed, GameSummaryEmbed, VictoryEmbed

from .client import Client

if TYPE_CHECKING:
    from .context import PerudoContext

class GameService():
    def __init__(self, ctx: 'PerudoContext'):
        self.ctx = ctx
        
        self.game_id = 0
        self.game_state = GameState.Terminated
        self.discord_players : Dict[int, Player] = {}

        self.game_setup : GameSetup = None
        self.round : Round = None

        self.bot_channel = ctx.bot.bot_channel
        self.voice_client : VoiceClient = None
        self.voice_channel : VoiceChannel = None

    async def resume_game(self, game_id) -> Round:
        self.game_id = game_id
        round_data = Client.resume_game(self.game_id)
        round = Round(round_data)
        await self._update_from_round(round)
        await self._send_out_dice(round)
        return round

    async def current_round(self) -> Round:
        round_data = Client.current_round(self.game_id)
        round = Round(round_data)
        await self._update_from_round(round)
        return round

    async def fetch_setup(self) -> GameSetup:
        setup_data = Client.fetch_setup(self.game_id)
        game_setup = GameSetup(setup_data)
        self._update_from_setup(game_setup)
        return game_setup
    
    async def create_game(self) -> GameSetup:
        setup_data = Client.create_game()
        game_setup = GameSetup(setup_data)
        self._update_from_setup(game_setup)
        return game_setup

    async def set_default_round_type(self, round_type) -> GameSetup:
        setup_data = Client.set_default_round_type(self.game_id, round_type)
        game_setup = GameSetup(setup_data)
        self._update_from_setup(game_setup)
        return game_setup
    
    async def add_player(self, user: Union[User, Member]) -> GameSetup:
        self.set_voice_channel(user)
        name = user.nick or user.name
        setup_data = Client.add_player(self.game_id, user.id, name, user.bot)
        game_setup = GameSetup(setup_data)
        self._update_from_setup(game_setup)
        return game_setup

    async def start_game(self):
        Client.start_game(self.game_id)
        self.game_state = GameState.InProgress
        await self._start_game_voice()
        return await self.start_round()

    async def start_round(self) -> Round:
        round_data = Client.start_round(self.game_id)
        round = Round(round_data)
        await self._update_from_round(round)
        await self._send_out_dice(round)
        return round

    async def bid_action(self, discord_id, quantity, pips) -> Round:
        round_data = Client.bid_action(self.game_id, self._player_id(discord_id), quantity, pips)
        round = Round(round_data)
        await self._update_from_round(round)
        self._play_notification('notify_pop.mp3')
        return round

    async def reverse_action(self, discord_id) -> Round:
        round_data = Client.reverse_action(self.game_id, self._player_id(discord_id))
        round = Round(round_data)
        await self._update_from_round(round)
        self._play_notification('notify_reverse.mp3')
        return round
    
    async def bet_action(self, discord_id, amount, bet_type, target_id) -> Round:
        round_data = Client.bet_action(self.game_id, self._player_id(discord_id), amount, bet_type, target_id)
        round = Round(round_data)
        await self._update_from_round(round)
        self._play_notification('notify_coins.mp3')
        return round

    async def liar_action(self, discord_id) -> Round:
        round_data = Client.liar_action(self.game_id, self._player_id(discord_id))
        round = Round(round_data)
        await self._update_from_round(round)
        self._play_notification('notify_long_pop.mp3')
        return round

    async def round_summary(self) -> RoundSummary:
        summary_data = Client.round_summary(self.game_id)
        summary = RoundSummary(summary_data)
        await self._update_from_round(summary.round)
        return summary
    
    async def end_game(self) -> GameSummary:
        await self._end_game_voice()
        summary_data = Client.end_game(self.game_id)
        game_summary = GameSummary(summary_data)

        winner = self.round.players[game_summary.winning_player_id]
        await self.ctx.send_delayed(embed=VictoryEmbed(winner))
        await self.ctx.send_delayed(embed=GameSummaryEmbed(game_summary))
    
    async def terminate(self):
        if self.voice_client is not None: await self.voice_client.disconnect()
        Client.terminate_game(self.game_id)
        self.game_state = GameState.Terminated
    
    async def _start_game_voice(self):
        try: self.voice_client = await self.voice_channel.connect()
        except: pass
        self._play_notification('notify_round_start.mp3')

    async def _end_game_voice(self):
        if self.voice_client is None: return
        self._play_notification('notify_game_over.mp3')
        await asyncio.sleep(5)
        await self.voice_client.disconnect()

    def set_voice_channel(self, member: Member):
        if member.voice is None: return
        if member.voice.channel is None: return
        self.voice_channel = member.voice.channel
    
    def _update_from_setup(self, game_setup: GameSetup):
        self.game_id = game_setup.game_id
        self.discord_players = game_setup.discord_players
        self.game_state = GameState.Setup
        self.game_setup = game_setup

    def _play_notification(self, source = 'notify.mp3'):
        if self.voice_client is None: return
        is_windows = getenv('IS_WINDOWS', 'False') == 'True'
        try:
            if is_windows: audio_source = discord.FFmpegPCMAudio(executable='./audio/ffmpeg/ffmpeg.exe', source=f'./audio/{source}')
            else: audio_source = discord.FFmpegPCMAudio(source=f'./audio/{source}')
            self.voice_client.play(audio_source)
        except: pass

    async def send_liar_result(self, round: Round): 
        liar = round.liar
        players = round.players
        losing_player = players[liar.losing_player_id]

        await self.ctx.clear_message(type=MessageType.Round)
        liar_call_embed = await self.ctx.send_delayed(embed=LiarCalledEmbed(liar, players), delay=0)
        await asyncio.sleep(3)
        await self.ctx.clear_message(type=MessageType.Bets)
        await liar_call_embed.edit(embed=LiarCalledEmbed(liar, players, show_actual=True))
        await self.ctx.send_delayed(embed=DamageDealtEmbed(liar, players), delay=1)
        
        if losing_player.lives <= 0:
            await self.ctx.send_delayed(embed=DefeatEmbed(liar, players), delay=1)
    
    async def _send_out_dice(self, round: Round):
        for player in round.players.values():
            if len(player.dice) <= 0: continue
            if player.is_bot:
                await self._send_bot_dice(player)
    
    async def _update_from_round(self, round: Round):
        self.round = round
        self.discord_players = round.discord_players
        self.game_state = GameState.InProgress if not round.is_final else GameState.Ended

    async def send_bot_updates(self, round):
        if self.ctx.bot.bot_channel is None:
            print("Warning: Bot channel is not accessible")
            return
        
        for discord_id, player in self.discord_players.items():
            if not player.is_bot: continue
            update = bot_update(round, self.ctx.channel.id, player.points)
            await self.bot_channel.send(f'{get_mention(discord_id)} update {update}')
    
    async def _send_bot_dice(self, player: Player):
        if self.bot_channel is None:
            print("Warning: Bot channel is not accessible")
            return
        
        await self.bot_channel.send(f'{get_mention(player.discord_id)} deal {bot_dice(player, self.ctx.channel.id)}')
    
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
    def num_players(self) -> int:
        return len(self.discord_players)
    
    @property
    def has_bots(self) -> bool:
        if self.discord_players is None: return False
        for player in self.discord_players.values():
            if player.lives > 0 and player.is_bot:
                return True
        return False