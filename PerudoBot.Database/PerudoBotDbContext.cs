using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PerudoBot.Database.Data
{
    public class PerudoBotDbContext : DbContext
    {
        private IConfiguration Configuration { get; set; }

        public PerudoBotDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerHand> PlayerHands { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Season> Seasons { get; set; }

        public DbSet<PointsLog> PointsLogs { get; set; }
        public DbSet<EloLog> EloLogs { get; set; }

        public DbSet<BidAction> BidActions { get; set; }
        public DbSet<LiarAction> LiarActions { get; set; }
        public DbSet<BetAction> BetActions { get; set; }
        public DbSet<ReverseAction> ReverseActions { get; set; }

        public DbSet<RoundNote> Notes { get;set; }

        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>()
                .HasDiscriminator(a => a.ActionType);

            modelBuilder.Entity<UserLog>()
                .HasDiscriminator(u => u.UserLogType);

            modelBuilder.Entity<Achievement>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<UserAchievement>()
                .HasIndex(ua => new { ua.UserId, ua.AchievementId })
                .IsUnique();

            modelBuilder.Entity<UserAchievement>()
                .HasIndex(ua => ua.IsNew);

            modelBuilder.Entity<Round>()
                .HasIndex(r => new { r.GameId, r.RoundNumber })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.DiscordId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Achievements)
                .WithMany(x => x.Users)
                .UsingEntity<UserAchievement>();

            modelBuilder.Entity<Player>()
                .HasMany(x => x.Rounds)
                .WithMany(x => x.Players)
                .UsingEntity<PlayerHand>();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Games)
                .WithMany(x => x.Users)
                .UsingEntity<Player>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration.GetConnectionString("Default");
            var expandedPath = Environment.ExpandEnvironmentVariables(connectionString);
            options.UseSqlite(expandedPath);
        }
    }
}
