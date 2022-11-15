using Microsoft.AspNetCore.Mvc;
using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.API.Filters;
using PerudoBot.API.Services;

namespace PerudoBot.API.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;
        private readonly UserService _userService;
        private readonly BetService _betService;

        public GameController(GameService gameService, UserService userService, BetService betService)
        {
            _gameService = gameService;
            _userService = userService;
            _betService = betService;
        }

        [HttpGet]
        [Route("game/round")]
        [RequireExistingGame]
        public IResult CurrentRoundState()
        {
            var response = _gameService.GetCurrentRound();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpGet]
        [Route("game/setup")]
        [RequireGameInSetup]
        public IResult CurrentGameSetup()
        {
            var response = _gameService.GetGameSetup();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("game/create")]
        public IResult CreateGame()
        {
            var response = _gameService.CreateGame(RoundType.Reverse);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("game/roundtype/{roundTypeId}")]
        [RequireGameInSetup]
        public IResult SetDefaultRoundType(int roundTypeId)
        {
            var response = _gameService.SetDefaultRoundType(roundTypeId);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }


        [HttpPost]
        [Route("game/start")]
        [RequireGameInSetup]
        public IResult StartGame()
        {
            var response = _gameService.StartGame();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("game/addplayer")]
        [RequireGameInSetup]
        public IResult AddPlayer(DiscordUser discordUser)
        {
            var user = _userService.GetUserFromDiscordUser(discordUser);
            var response = _gameService.AddUserAsPlayer(user);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("game/terminate")]
        [RequireExistingGame]
        public IResult TerminateGame()
        {
            _gameService.TerminateGame();

            return Results.Ok(new { data = "" });
        }

        [HttpPost]
        [Route("game/bet")]
        [RequirePlayerInGame]
        public IResult BetAction(BetAttempt bet)
        {
            var game = _gameService.GetActiveGame();
            var player = _gameService.GetActivePlayer();

            var response = _betService.Bet(game, player, bet.Amount, bet.Type, bet.TargetBidId);
            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }

        [HttpPost]
        [Route("game/bid")]
        [RequirePlayerInGame]
        public IResult BidAction(BidAttempt bid)
        {
            var response = _gameService.AddBidAction(bid.Quantity, bid.Pips);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = _gameService.GetCurrentRound() });
        }

        [HttpPost]
        [Route("game/liar")]
        [RequirePlayerInGame]
        public IResult LiarAction()
        {
            var response = _gameService.AddLiarAction();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }            

            return Results.Ok(new { data = _gameService.GetCurrentRoundSummary() });
        }

        [HttpPost]
        [Route("game/reverse")]
        [RequirePlayerInGame]
        public IResult ReverseAction()
        {
            var response = _gameService.AddReverseAction();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = _gameService.GetCurrentRound() });
        }

        [HttpPost]
        [Route("game/end")]
        [RequireGameInProgress]
        public IResult EndGame()
        {
            _gameService.EndGame();

            return Results.Ok(new { data = _gameService.GameSummary() });
        }

        [HttpPost]
        [Route("game/newround")]
        [RequireGameInProgress]
        public IResult StartNewRound()
        {
            var response = _gameService.StartNewRound();

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }


        [HttpPost]
        [Route("game/note")]
        [RequirePlayerInGame]
        public IResult RoundNote(NoteAttempt note)
        {
            var response = _gameService.AddRoundNote(note.Text);

            if (!response.RequestSuccess)
            {
                return Results.BadRequest(new { error = response.ErrorMessage });
            }

            return Results.Ok(new { data = response });
        }
    }
}
