using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Helpers;
using PerudoBot.Database.Data;

namespace PerudoBot.API.Services
{
    public class GameService
    {
        private Player _activePlayer { get; set; }
        private Game _activeGame { get; set; }

        private readonly PerudoBotDbContext _db;
        private readonly BetService _betService;
        private readonly EloService _eloService;
        private readonly UserService _userService;
        private readonly ItemService _itemService;
        private readonly AuctionService _auctionService;
        private readonly AchievementService _achievementService;

        public GameService(
            PerudoBotDbContext context, 
            BetService betService, 
            EloService eloService, 
            UserService userService, 
            ItemService itemService,
            AuctionService auctionService, 
            AchievementService achievementService)
        {
            _db = context;
            _betService = betService;
            _eloService = eloService;
            _userService = userService;
            _itemService = itemService;
            _auctionService = auctionService;
            _achievementService = achievementService;
        }

        public Game LoadActiveGame(int gameId)
        {
            _activeGame = _db.Games
                .Where(x => x.Id == gameId)
                .Include(x => x.Players)
                    .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Rattles)
                .Include(x => x.Players)
                    .ThenInclude(x => x.User)
                        .ThenInclude(x => x.UserItems)
                            .ThenInclude(x => x.Item)
                .Include(x => x.Rounds)
                    .ThenInclude(x => x.Actions)
                .Include(x => x.Rounds)
                    .ThenInclude(x => x.PlayerHands)
                .SingleOrDefault();

            return _activeGame;
        }

        public Player LoadActivePlayer(int playerId)
        {
            _activePlayer = _db.Players.SingleOrDefault(x => x.Id == playerId);
            return _activePlayer;
        }

        public Game GetActiveGame()
        {
            return _activeGame;
        }

        public Player GetActivePlayer()
        {
            return _activePlayer;
        }

        private GameSetupDto GetGameSetupDto(Game game)
        {
            if (!_auctionService.IsAvailable())
            {
                return game.ToGameSetupDto();
            }

            var dailyAuction = _auctionService.GetDailyAuction();

            if (dailyAuction.State != (int)AuctionState.Setup)
            {
                return game.ToGameSetupDto();
            }

            return game.ToGameSetupDto(dailyAuction);
        }

        public GameSetupDto SetDefaultRoundType(int roundType)
        {
            _activeGame.DefaultRoundType = roundType;
            _db.SaveChanges();

            return GetGameSetupDto(_activeGame);
        }

        public GameSetupDto GetGameSetup()
        {
            return GetGameSetupDto(_activeGame);
        }

        public GameSetupDto CreateGame(RoundType roundType)
        {
            var season = new Season();
            _db.Seasons.Add(season);
            _db.SaveChanges();

            var game = new Game
            {
                State = (int) GameState.Setup,
                Players = new List<Player>(),
                Rounds = new List<Round>(),
                DefaultRoundType = (int) roundType,
                SeasonId = season.Id
            };

            _db.Games.Add(game);
            _db.SaveChanges();

            return GetGameSetupDto(game);
        }

        public Response StartGame()
        {
            if (_activeGame.Players.Count < 2)
            {
                return Responses.Error("Need at least 2 players");
            }

            _activeGame.Players = _activeGame.Players.Shuffle();
            _activeGame.State = (int)GameState.InProgress;
            _db.SaveChanges();

            return Responses.OK();
        }

        public RoundDto StartNewRound()
        {
            var round = new Round
            {
                GameId = _activeGame.Id,
                RoundType = _activeGame.DefaultRoundType,
                RoundNumber = _activeGame.Rounds.Count + 1,
            };

            if (_activeGame.Rounds.Count > 0)
            {
                round.StartingPlayerId = _activeGame.Players
                    .GetStartingPlayerId(_activeGame.LatestRound.Liar.LosingPlayerId);
            }
            else
            {
                round.StartingPlayerId = _activeGame.Players.GetStartingPlayerId();
            }

            round.ActivePlayerId = round.StartingPlayerId;

            _db.Rounds.Add(round);
            _db.SaveChanges();

            // Deal dice by creating RoundPlayers
            foreach (var player in _activeGame.ActivePlayers)
            {
                round.PlayerHands.Add(player.DealHand(round));
            }

            _db.SaveChanges();

            return round.ToRoundDto();
        }

        public GameSetupDto AddUserAsPlayer(User user)
        {
            if (_activeGame.Players.Any(x => x.UserId == user.Id))
            {
                return new GameSetupDto { ErrorMessage = "User already added as player", RequestSuccess = false };
            }

            var player = new Player
            {
                UserId = user.Id,
                GameId = _activeGame.Id,
                Lives = GameConstants.STARTING_LIVES,
                TurnOrder = _activeGame.Players.Count
            };

            _db.Players.Add(player);
            _db.SaveChanges();

            return GetGameSetupDto(_activeGame);
        }

        public Response AddBidAction(int quantity, int pips)
        {
            var currentRound = _activeGame.LatestRound;

            if (currentRound.IsCompleted)
            {
                return Responses.Error("Round is completed");
            }

            if (currentRound.ActivePlayerId != _activePlayer?.Id)
            {
                return Responses.Error("Player not currently active");
            }

            var latestBid = currentRound.LatestBid;

            if (!latestBid.IsValidFollowUp(quantity, pips))
            {
                return Responses.Error("Invalid bid attempt");
            }

            var roundPlayer = currentRound.PlayerHands
                .SingleOrDefault(x => x.PlayerId == _activePlayer.Id);

            var bidAction = new BidAction
            {
                ParentActionId = latestBid?.Id,
                RoundId = currentRound.Id,
                PlayerId = _activePlayer.Id,
                PlayerHandId = roundPlayer.Id,
                Quantity = quantity,
                Pips = pips
            };

            currentRound.ActivePlayerId = _activeGame.Players.GetNextActivePlayerId(currentRound.ActivePlayerId);

            _db.BidActions.Add(bidAction);
            _db.SaveChanges();

            return Responses.OK();
        }

        public Response AddReverseAction()
        {
            var currentRound = _activeGame.LatestRound;

            if (currentRound.IsCompleted)
            {
                return Responses.Error("Round is completed");
            }

            if (currentRound.ActivePlayerId != _activePlayer?.Id)
            {
                return Responses.Error("Player not currently active");
            }

            if (currentRound.Actions.OfType<ReverseAction>().Any())
            {
                return Responses.Error("Reverse can only be used once per round");
            }

            var roundPlayer = currentRound.PlayerHands
                .SingleOrDefault(x => x.PlayerId == _activePlayer.Id);

            var reverseAction = new ReverseAction
            {
                ParentActionId = currentRound.LatestAction?.Id,
                RoundId = currentRound.Id,
                PlayerId = _activePlayer.Id,
                PlayerHandId = roundPlayer.Id
            };

            _activeGame.Players = _activeGame.Players.Reverse();

            _db.ReverseActions.Add(reverseAction);
            _db.SaveChanges();

            return Responses.OK();
        }

        public RoundDto GetCurrentRound()
        {
            var round = _activeGame.LatestRound.ToRoundDto();
            return round;
        }

        public RoundDto ResumeGame()
        {
            if (_activeGame.State != (int) GameState.InProgress)
            {
                return new RoundDto { ErrorMessage = "Game is not in progress" };
            }

            var round = _activeGame.LatestRound.ToRoundDto();
            if (round.Liar == null) return round;

            if (round.ActivePlayerCount <= 1)
            {
                return new RoundDto { ErrorMessage = "Game is not in progress" };
            }

            return StartNewRound();
        }

        public RoundSummaryDto GetCurrentRoundSummary()
        {
            return new RoundSummaryDto
            {
                Round = _activeGame.LatestRound.ToRoundDto(),
                Achievements = _achievementService.GetNewAchievements()
            };
        }

        public Response AddLiarAction()
        {
            var currentRound = _activeGame.LatestRound;

            if (!currentRound.Players.Any(x => x.Id == _activePlayer.Id))
            {
                return Responses.Error("Player not in this round");
            }

            if (currentRound.IsCompleted)
            {
                return Responses.Error("Round is completed");
            }

            if (currentRound.LatestBid == null)
            {
                return Responses.Error("No bids have been placed");
            }

            var latestBid = currentRound.LatestBid;

            var playerThatCalledLiar = currentRound.PlayerHands
                .SingleOrDefault(x => x.PlayerId == _activePlayer.Id);

            var allDice = currentRound.PlayerHands.GetAllDice();
            var actualQuantity = latestBid.GetActualQuantity(allDice);

            var liarAction = new LiarAction
            {
                RoundId = currentRound.Id,
                ParentActionId = latestBid.Id,
                PlayerId = _activePlayer.Id,
                PlayerHandId = playerThatCalledLiar.Id,
                ActualQuantity = actualQuantity
            };

            if (actualQuantity < latestBid.Quantity)
            {
                // Liar caller was correct
                liarAction.LosingPlayerId = latestBid.PlayerId;
                liarAction.LivesLost = currentRound.GetLivesLost(latestBid.Quantity - actualQuantity);
                DecrementLives(currentRound, liarAction);
            }
            else
            {
                // Bidder was correct
                liarAction.LosingPlayerId = playerThatCalledLiar.PlayerId;
                liarAction.LivesLost = currentRound.GetLivesLost(actualQuantity - latestBid.Quantity + 1);
                DecrementLives(currentRound, liarAction);
            }

            _db.LiarActions.Add(liarAction);

            _db.SaveChanges();

            _betService.AwardBetPoints(currentRound);

            _achievementService.CheckRoundAchievements(_activeGame, currentRound);

            foreach (var player in _activeGame.ActivePlayers)
            {
                _userService.AddPassiveIncome(player.User, GameConstants.POINTS_EARNED_PER_ROUND, _activeGame);
            }

            return Responses.OK();
        }

        private void DecrementLives(Round round, LiarAction liar)
        {
            var playerThatLostDice = _activeGame.Players.Single(x => x.Id == liar.LosingPlayerId);

            if (playerThatLostDice.Lives > liar.LivesLost)
            {
                playerThatLostDice.Lives -= liar.LivesLost;
            }
            else
            {
                playerThatLostDice.Lives = 0;
                playerThatLostDice.RoundEliminated = round.RoundNumber;
            }
        }

        public Response EndGame()
        {
            var winningPlayer = _activeGame.Players.Single(x => x.Lives > 0);
            winningPlayer.RoundEliminated = _activeGame.LatestRound.RoundNumber + 1;

            _activeGame.WinningPlayerId = winningPlayer.Id;
            _activeGame.State = (int) GameState.Ended;

            _eloService.UpdateElo(_activeGame);
            _achievementService.CheckGameAchievements(_activeGame);
            _itemService.ResolveGameItemDrops(_activeGame);

            _db.SaveChanges();

            return Responses.OK();
        }

        public Response AddRoundNote(string text)
        {
            text = text.StripSpecialCharacters();
            if (text.Length > 256) return Responses.Error("Note too long");

            _db.Notes.Add(new RoundNote
            {
                UserId = _activePlayer.UserId,
                RoundId = _activeGame.LatestRound.Id,
                Text = text,
            });

            _db.SaveChanges();

            return Responses.OK();
        }

        public GameDto GameSummary()
        {
            return new GameDto
            {
                GameId = _activeGame.Id,
                WinningPlayerId = _activeGame.WinningPlayerId,
                BetPointsChanges = _activeGame.Players
                    .Select(player => player.
                        ToPlayerPointsChange(_userService
                            .GetPointsChangeForGame(player.User, _activeGame.Id, PointsLogType.Gambling)))
                    .Where(x => x.PointsChange != 0)
                    .OrderByDescending(x => x.PointsChange)
                    .ToList(),
                EloChanges = _activeGame.Players
                    .Select(player => player
                        .ToPlayerEloChange(_userService
                            .GetEloChangeForGame(player.User, _activeGame.Id)))
                    .OrderByDescending(x => x.EloChange)
                    .ToList(),
                Notes = _db.Notes
                    .Where(x => x.Round.GameId == _activeGame.Id)
                    .Select(x => x.ToGameNote())
                    .ToList(),
                Achievements = _achievementService.GetNewAchievements(),
                ItemDrops = _db.ItemDrops
                    .Where(x => x.GameId == _activeGame.Id)
                    .Select(x => x.ToItemDropDto())
                    .ToList()
            };
        }

        public Response TerminateGame()
        {
            _activeGame.State = (int)GameState.Terminated;

            _db.SaveChanges();
            return Responses.OK();
        }
    }
}
