using PerudoBot.API.Constants;
using PerudoBot.API.DTOs;
using PerudoBot.Database.Data;
using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Helpers;

namespace PerudoBot.API.Services
{
    public class UserService
    {
        private PerudoBotDbContext _db;

        public UserService(PerudoBotDbContext context)
        {
            _db = context;
        }

        public User GetUserFromDiscordUser(DiscordUser discordUser)
        {
            var existingUser = _db.Users
                .SingleOrDefault(x => x.DiscordId == discordUser.DiscordId);

            if (existingUser != null)
            {
                if (existingUser.Name != discordUser.Name)
                {
                    existingUser.Name = discordUser.Name;
                    _db.SaveChanges();
                }

                if (existingUser.IsBot != discordUser.IsBot)
                {
                    existingUser.IsBot = discordUser.IsBot;
                    _db.SaveChanges();
                }

                return existingUser;
            }

            var user = new User
            {
                DiscordId = discordUser.DiscordId,
                IsBot = discordUser.IsBot,
                Name = discordUser.Name,
                Elo = 1200,
                Points = 100
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }

        public void AddPassiveIncome(User user, int points, Game game = null)
        {
            var newPoints = user.Points + points;
            UpdatePoints(user, newPoints, PointsLogType.PassiveIncome, game);
        }

        public void RemoveBetPoints(User user, int points, Game game = null)
        {
            var newPoints = user.Points - points;
            UpdatePoints(user, newPoints, PointsLogType.Gambling, game);
        }

        public void AddBetPoints(User user, int points, Game game = null)
        {
            var newPoints = user.Points + points;
            UpdatePoints(user, newPoints, PointsLogType.Gambling, game);
        }

        private void UpdatePoints(User user, int points, PointsLogType pointsLogType, Game game = null)
        {
            var pointChange = points - user.Points;
            user.Points += pointChange;

            _db.PointsLogs.Add(new PointsLog
            {
                UserId = user.Id,
                GameId = game?.Id,
                PointsChange = pointChange,
                PointsLogTypeId = (int)pointsLogType
            });

            _db.SaveChanges();
        }

        public void UpdateElo(User user, int elo, Game game = null)
        {
            var eloChange = elo - user.Elo;
            user.Elo += eloChange;

            _db.EloLogs.Add(new EloLog
            {
                UserId = user.Id,
                GameId = game?.Id,
                EloChange = eloChange
            });

            _db.SaveChanges();
        }

        public int GetEloChangeForGame(User user, int gameId)
        {
            return _db.EloLogs
                .Where(x => x.GameId == gameId)
                .Where(x => x.UserId == user.Id)
                .Sum(x => x.EloChange);
        }

        public int GetPointsChangeForGame(User user, int gameId, PointsLogType? pointsType = null)
        {
            var pointsChanges = _db.PointsLogs
                .Where(x => x.GameId == gameId)
                .Where(x => x.UserId == user.Id);

            if (pointsType != null)
            {
                pointsChanges = pointsChanges
                    .Where(x => x.PointsLogTypeId == (int)pointsType);
            }

            return pointsChanges.Sum(x => x.PointsChange);
        }

        public LadderInfoDto GetLadderInfo()
        {
            var ladderEntries = _db.Users
                .Include(x => x.Players)
                    .ThenInclude(x => x.Game)
                .OrderByDescending(x => x.Elo)
                .Select(x => x.ToLadderEntryDto())
                .ToList();

            return new LadderInfoDto
            {
                LadderEntries = ladderEntries
            };
        }
    }
}
