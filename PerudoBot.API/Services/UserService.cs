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

        public User GetUserFromDiscordId(ulong discordId)
        {
            return _db.Users
                .SingleOrDefault(x => x.DiscordId == discordId);
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
                Points = 1000
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
            var activeUsers = _db.Games
                .Where(x => x.State == (int)GameState.Ended)
                .Include(x => x.Users)
                .AsEnumerable()
                .TakeLast(20)
                .SelectMany(x => x.Users)
                .Distinct();

            var ladderEntries = activeUsers
                .Select(x => x.ToLadderEntryDto())
                .ToList();

            return new LadderInfoDto { LadderEntries = ladderEntries };
        }

        public List<Rattle> ListRattles()
        {
            if (!_db.Rattles.Any()) return new List<Rattle>();
            return _db.Rattles.ToList();
        }

        public Response UpdateRattle(RattleUpdate rattleUpdate)
        {
            var user = GetUserFromDiscordId(rattleUpdate.DiscordId);

            if (user == null)
            {
                return Responses.Error("User is not recognized");
            }

            var rattle = _db.Rattles
                .Where(x => x.UserId == user.Id)
                .Where(x => x.RattleType == (int)rattleUpdate.RattleType)
                .Where(x => x.RattleContentType == (int)rattleUpdate.RattleContentType)
                .SingleOrDefault();

            if (rattle == null)
            {
                _db.Rattles.Add(new Rattle
                {
                    UserId = user.Id,
                    RattleType = (int)rattleUpdate.RattleType,
                    RattleContentType = (int)rattleUpdate.RattleContentType,
                    Content = rattleUpdate.Content
                });
            }
            else
            {
                rattle.Content = rattleUpdate.Content;
            }

            _db.SaveChanges();

            return Responses.OK();
        }

        public UserProfileDto GetUserProfile(ulong discordId)
        {
            var user = _db.Users
                .Where(x => x.DiscordId == discordId)
                .Include(x => x.UserAchievements)
                    .ThenInclude(x => x.Achievement)
                        .ThenInclude(x => x.Users)
                .Include(x => x.Games)
                    .ThenInclude(x => x.Players)
                .Include(x => x.Games)
                    .ThenInclude(x => x.UserLogs)
                .SingleOrDefault();

            if (user == null) 
            {
                return new UserProfileDto { RequestSuccess = false, ErrorMessage = "User is not recognized" };
            }

            var userProfile = user.ToUserProfileDto();
            CalculateUserRanks(userProfile);

            return userProfile;
        }

        private void CalculateUserRanks(UserProfileDto userProfile)
        {
            foreach (var user in _db.Users)
            {
                if (user.Elo > userProfile.Elo) userProfile.EloRank++;
                if (user.Points > userProfile.Points) userProfile.PointsRank++;
                if (user.AchievementScore > userProfile.Score) userProfile.ScoreRank++;
            }
        }
    }
}
