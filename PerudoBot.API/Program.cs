using Microsoft.EntityFrameworkCore;
using PerudoBot.API.Constants;
using PerudoBot.API.Services;
using PerudoBot.Database.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<AchievementService>()
    .AddScoped<GameService>()
    .AddScoped<UserService>()
    .AddScoped<BetService>()
    .AddScoped<EloService>()
    .AddDbContext<PerudoBotDbContext>()
    .AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PerudoBotDbContext>();
    //db.Database.EnsureDeleted();
    db.Database.Migrate();

    var achievementService = scope.ServiceProvider.GetRequiredService<AchievementService>();

    var completedGames = db.Games
        .Where(x => x.State == (int)GameState.Ended)
        .Include(x => x.Players)
            .ThenInclude(x => x.User)
        .Include(x => x.Rounds)
            .ThenInclude(x => x.Actions)
        .Include(x => x.Rounds)
            .ThenInclude(x => x.PlayerHands);

    Console.WriteLine("Starting to check achievements");
    foreach (var game in completedGames)
    {
        foreach (var round in game.Rounds)
        {
            achievementService.CheckRoundAchievements(game, round);
        }
        achievementService.CheckGameAchievements(game);
    }
    Console.WriteLine("Completed check achievements");
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
