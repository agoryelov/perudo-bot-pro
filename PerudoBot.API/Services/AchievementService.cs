using PerudoBot.Database.Data;
using PerudoBot.API.Achievements;
using Microsoft.EntityFrameworkCore;
using PerudoBot.API.DTOs;
using PerudoBot.API.Helpers;

namespace PerudoBot.API.Services
{
    public class AchievementService
    {
        private readonly PerudoBotDbContext _db;
        public AchievementService(PerudoBotDbContext context)
        {
            _db = context;
        }
        
        public List<UserAchievementDto> GetAchievementsForUser(User user)
        {
            return _db.UserAchievements
                .Where(x => x.UserId == user.Id)
                .Include(x => x.Achievement)
                .Select(x => x.ToUserAchievementDto())
                .ToList();
        }

        public List<AchievementDto> GetAchievements()
        {
            return _db.Achievements
                .Include(x => x.UserAchievements)
                    .ThenInclude(x => x.User)
                .OrderByDescending(x => x.UserAchievements.Count())
                .Select(x => x.ToAchievementDto())
                .ToList();
        }

        public List<UserAchievementDto> GetNewAchievements()
        {
            var newAchivements = _db.UserAchievements
                .Where(x => x.IsNew)
                .Include(x => x.Achievement)
                .ToList();

            var achievementsDto = newAchivements
                .Select(x => x.ToUserAchievementDto())
                .ToList();

            foreach (var achievement in newAchivements)
            {
                achievement.IsNew = false;
            }

            _db.SaveChangesAsync();

            return achievementsDto;
        }

        public void CheckRoundAchievements(Game game, Round round)
        {
            var players = game.Players.ToList();
            CheckAchievements(AchievementData.RoundAchievements, players, game, round);
        }

        public void CheckGameAchievements(Game game)
        {
            var players = game.Players.ToList();
            CheckAchievements(AchievementData.GameAchievements, players, game);
        }

        private void CheckAchievements(List<AchievementCheck> achievements, List<Player> players, Game game, Round round = null)
        {
            foreach (var achievementCheck in achievements)
            {
                var achievement = GetOrCreateAchievement(achievementCheck);

                foreach (var player in players)
                {
                    if (UserHasAchievement(player.User, achievement)) continue;
                    if (!achievementCheck.Evaluate(player, game, round)) continue;
                    AddUserAchievement(player.User, achievement);
                }
            }
            _db.SaveChanges();
        }

        private bool UserHasAchievement(User user, Achievement achievement)
        {
            return _db.UserAchievements.Any(x => x.Id == user.Id && x.AchievementId == achievement.Id);
        }

        private void AddUserAchievement(User user, Achievement achievement)
        {
            var hasAchievement = _db.UserAchievements.Any(x => x.UserId == user.Id && x.AchievementId == achievement.Id);
            if (hasAchievement) return;

            _db.UserAchievements.Add(new UserAchievement 
            {
                AchievementId = achievement.Id,
                UserId = user.Id
            });
        }

        private Achievement GetOrCreateAchievement(AchievementCheck check)
        {
            var achievement = _db.Achievements.SingleOrDefault(x => x.Name == check.Name);
            if (achievement != null) return achievement;

            achievement = check.ToAchievement();

            _db.Achievements.Add(achievement);
            _db.SaveChanges();

            return achievement;
        }
    }
}
